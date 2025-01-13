using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace ThemeOptions.Tools
{
    class DynamicEventHandler
    {
        static public Delegate AddEventHandler(dynamic eventOwner, string eventName, object methodOwner, string methodName)
        {
            EventInfo eventInfo = eventOwner.GetType().GetEvent(eventName);
            Type delegateType = eventInfo.EventHandlerType;
            MethodInfo invokeMethod = delegateType.GetMethod("Invoke");

            // Get the parameter types from the Invoke method
            ParameterInfo[] parameters = invokeMethod.GetParameters();
            Type[] parameterTypes = new Type[parameters.Length + 1];
            parameterTypes[0] = methodOwner.GetType();
            for (int i = 1; i < parameters.Length + 1; i++)
            {
                parameterTypes[i] = parameters[i - 1].ParameterType;
            }

            // Create the dynamic method
            DynamicMethod dynamicMethod = new DynamicMethod(
                $"{eventName}DynamicHandler",
                null,
                parameterTypes,
                typeof(Gamepad).Module,
                true);


            // Get an ILGenerator and emit the IL for the dynamic method
            ILGenerator il = dynamicMethod.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Call, methodOwner.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic));
            il.Emit(OpCodes.Ret);


            // Create the delegate
            Delegate handler = dynamicMethod.CreateDelegate(delegateType, methodOwner);

            // Add the event handler
            eventInfo.AddEventHandler(eventOwner, handler);
            return handler;
        }
    }
}