using IdentityService.Domain.Interfaces;
using SharedKernel.Base;
using SharedKernel.Messaging;
using SharedKernel.Multimedia;
using VQAService.Domain.Entities.Conversations;
using VQAService.Domain.Entities.Input;
using VQAService.Domain.Entities.Input.Options;
using VQAService.Domain.Entities.Input.Options.Language;
using VQAService.Domain.Entities.Output;
using VQAService.Domain.Interfaces;

namespace VQAService.Application.IC;

public sealed class ProcessICQueryHandler(
    IVQAServiceProvider serviceProvider,
    IConversationsRepository conversationsRepository,
    IUserContext userContext) : IQueryHandler<ProcessICQuery, ICOutput>
{
    public async Task<Result<ICOutput>> Handle(ProcessICQuery query, CancellationToken cancellationToken)
    {
        #region Process inputs
        var options = new VQAOptions();
        options.Model = query.Model ?? options.Model;
        if (query.LanguageCode is not null &&
            VQALanguages.VQASupportedLanguagesMap.TryGetValue(query.LanguageCode, out var lang))
        {
            options.Language = lang;
        }

        var createInputResult = ICInput.Create(query.ConversationId, options);
        if (createInputResult.IsFailure)
        {
            return Result.Failure<ICOutput>(createInputResult.Error);
        }
        var icInput = createInputResult.Value;
        #endregion

        #region Check service availability
        var isServiceAvailable = await serviceProvider.IsAvailable();
        if (!isServiceAvailable)
        {
            return Result.Failure<ICOutput>(
                new Error("VQA.SERVICE_NOT_AVAILABLE", "VQA service is not available currently", ErrorType.Conflict)
                );
        }
        #endregion

        #region Get Conversation
        var getConversationResult = await conversationsRepository.GetById(icInput.ConversationId);
        if (getConversationResult.IsFailure)
        {
            return Result.Failure<ICOutput>(getConversationResult.Error);
        }
        var conversation = getConversationResult.Value;
        if (conversation.UserId != userContext.UserId)
        {
            return Result.Failure<ICOutput>(Error.Conflict("VQA.Forbidden", "It's not allowed to access required conversation"));
        }
        #endregion

        #region Perform IC
        var processICResult = await serviceProvider.ProcessIC(conversation.Image);
        if (processICResult.IsFailure)
        {
            return Result.Failure<ICOutput>(processICResult.Error);
        }
        var icOutput = processICResult.Value;
        #endregion

        #region Update conversation
        var newHistory = conversation.History;
        newHistory.Add(new HistoryItem { Question = "Caption this image", Answer = icOutput });
        var updateConversationHistoryResult = await conversationsRepository.UpdateHistory(conversation, newHistory);
        if (updateConversationHistoryResult.IsFailure)
        {
            return Result.Failure<ICOutput>(updateConversationHistoryResult.Error);
        }
        #endregion

        var result = new ICOutput { Caption = icOutput, ConversationId = conversation.Id };
        return result;

    }
}
