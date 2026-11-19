using System;
using Libraries.DNV.InputTemplate;
using Libraries.DNV.InputTemplate.Template;

namespace Libraries.DNV.Extensions
{
    public static class ButtonModelExtensions
    {
        public static bool IsInputMatchesType(this ButtonModel model, InputType type)
        {
            return type switch
            {
                InputType.Pressed => model.IsPressed,
                InputType.Released => model.IsReleased,
                InputType.Hold => model.IsHold,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}
