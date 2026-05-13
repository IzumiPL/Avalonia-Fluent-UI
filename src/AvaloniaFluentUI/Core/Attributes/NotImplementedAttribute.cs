using System;

namespace AvaloniaFluentUI.Core.Attributes;

/// <summary>
/// Marks that a property or class has not been implemented in the AvaloniaFluentUI version
/// </summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
public class NotImplementedAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotImplementedException"/>  class.
    /// </summary>
    public NotImplementedAttribute() { }
}
