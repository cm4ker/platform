﻿using System;

namespace ZenPlatform.DataComponent.Interfaces
{
    /// <summary>
    /// Обязательный инетфейс для сущностей, на которые могут ссылаться 
    /// </summary>
    public interface IReference
    {
        int Type { get; }

        Guid Id { get; }
    }
}