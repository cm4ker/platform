using System;

namespace Aquila.Core;

/// <summary>
/// User exception
/// </summary>
public class AqException : Exception
{
    public AqException() : base()
    {
        
    }
    
    public AqException(string message) : base(message)
    {
        
    }
}