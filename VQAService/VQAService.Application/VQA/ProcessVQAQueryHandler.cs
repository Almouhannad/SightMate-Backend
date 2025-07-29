using SharedKernel.Base;
using SharedKernel.Messaging;
using VQAService.Domain.Entities.Input;
using VQAService.Domain.Entities.Input.Options;
using VQAService.Domain.Entities.Input.Options.Language;
using VQAService.Domain.Entities.Output;
using VQAService.Domain.Interfaces;
using VQAService.Domain.Entities.Conversations;
using IdentityService.Domain.Interfaces;

namespace VQAService.Application.VQA;

public sealed class ProcessVQAQueryHandler(IVQAServiceProvider serviceProvider,
    IConversationsRepository conversationsRepository,
    IUserContext userContext) : IQueryHandler<ProcessVQAQuery, VQAOutput>
{
    public async Task<Result<VQAOutput>> Handle(ProcessVQAQuery query, CancellationToken cancellationToken)
    {
        #region Process inputs
        var options = new VQAOptions();
        options.Model = query.Model ?? options.Model;
        if (query.LanguageCode is not null &&
            VQALanguages.VQASupportedLanguagesMap.TryGetValue(query.LanguageCode, out var lang))
        {
            options.Language = lang;
        }

        var createInputResult = VQAInput.Create(query.ConversationId, query.Question, options);
        if (createInputResult.IsFailure)
        {
            return Result.Failure<VQAOutput>(createInputResult.Error);
        }
        var vqaInput = createInputResult.Value;
        #endregion

        #region Check service availability
        var isServiceAvailable = await serviceProvider.IsAvailable();
        if (!isServiceAvailable)
        {
            return Result.Failure<VQAOutput>(
                new Error("VQA.SERVICE_NOT_AVAILABLE", "VQA service is not available currently", ErrorType.Conflict)
                );
        }
        #endregion

        #region Get Conversation
        var getConversationResult = await conversationsRepository.GetById(vqaInput.ConversationId);
        if (getConversationResult.IsFailure)
        {
            return Result.Failure<VQAOutput>(getConversationResult.Error);
        }
        var conversation = getConversationResult.Value;
        if (conversation.UserId != userContext.UserId)
        {
            return Result.Failure<VQAOutput>(Error.Conflict("VQA.Forbidden", "It's not allowed to access required conversation"));
        }
        #endregion

        #region Perform VQA
        var processVQAResult = await serviceProvider.ProcessVQA(conversation.Image, vqaInput.Question, conversation.History);
        if (processVQAResult.IsFailure)
        {
            return Result.Failure<VQAOutput>(processVQAResult.Error);
        }
        var vqaResult = processVQAResult.Value;
        #endregion

        #region Update conversation history
        var newHistory = conversation.History;
        newHistory.Add(new HistoryItem { Question = vqaInput.Question, Answer = vqaResult });
        var updateConversationHistoryResult = await conversationsRepository.UpdateHistory(conversation, newHistory);
        if (updateConversationHistoryResult.IsFailure)
        {
            return Result.Failure<VQAOutput>(updateConversationHistoryResult.Error);
        }
        #endregion

        return new VQAOutput { Answer = vqaResult };
    }

}
