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

internal sealed class ProcessICQueryHandler(
    IVQAServiceProvider serviceProvider,
    IConversationsRepository conversationsRepository) : IQueryHandler<ProcessICQuery, ICOutput>
{
    public async Task<Result<ICOutput>> Handle(ProcessICQuery query, CancellationToken cancellationToken)
    {
        #region Process inputs
        var image = new Image { Bytes = query.ImageBytes, Metadata = query.ImageMetadata };

        var options = new VQAOptions();
        options.Model = query.Model ?? options.Model;
        if (query.LanguageCode is not null &&
            VQALanguages.VQASupportedLanguagesMap.TryGetValue(query.LanguageCode, out var lang))
        {
            options.Language = lang;
        }

        var createInputResult = ICInput.Create(image, options);
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

        #region Perform IC
        var processICResult = await serviceProvider.ProcessIC(icInput.Image);
        if (processICResult.IsFailure)
        {
            return Result.Failure<ICOutput>(processICResult.Error);
        }
        var icOutput = processICResult.Value;
        #endregion

        #region Create conversation
        // TODO: Handle UserId
        var history = new HistoryItem { Question = "Caption this image", Answer = icOutput };
        Conversation conversation = new() { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Image = icInput.Image, History = [history] };
        var createConversationResult = await conversationsRepository.Create(conversation);
        if (createConversationResult.IsFailure)
        {
            return Result.Failure<ICOutput>(createConversationResult.Error);

        }
        conversation = createConversationResult.Value;
        #endregion

        var result = new ICOutput { Caption = icOutput, ConversationId = conversation.Id };
        return result;

    }
}
