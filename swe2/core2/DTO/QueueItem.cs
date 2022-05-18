using System;

namespace core.DTO
{
	public record QueueItem(Guid CorrelationId, dynamic Payload);
}

