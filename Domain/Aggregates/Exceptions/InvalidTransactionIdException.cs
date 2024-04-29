﻿using Domain.Common;

namespace Domain.Aggregates.Exceptions;

public class InvalidTransactionIdException : BadRequestException
{
    public InvalidTransactionIdException(Guid transactionId)
        : base($"transactionId: '{transactionId}' is invalid.")
    {
    }
    
    public InvalidTransactionIdException(string transactionId)
        : base($"transactionId: '{transactionId}' is invalid.")
    {
    }
}