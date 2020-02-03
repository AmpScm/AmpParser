using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Amp.Linq
{
    public static partial class AmpQueryable
    {

        /// <summary>
        /// Get the method matching the specification
        /// </summary>
        /// <typeparam name="TFunc"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="instance"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static TFunc GetMethod<TFunc>(this Type type, string name, object instance, BindingFlags flags)
            where TFunc : Delegate
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (instance == null)
                flags |= BindingFlags.Static;
            else
                flags |= BindingFlags.Instance;

            MethodInfo invokeInfo = typeof(TFunc).GetMethod(nameof(Action.Invoke));
            ParameterInfo[] parameters = invokeInfo.GetParameters();
            Type[] argTypes = parameters.Select(x => x.ParameterType).ToArray();

            ParameterModifier[] modifiers = null;

            if (parameters.Any(x => x.IsOut))
            {
                ParameterModifier mod = new ParameterModifier(parameters.Length);
                for (int i = 0; i < parameters.Length; i++)
                {
                    mod[i] = parameters[i].IsOut;
                }
                modifiers = new ParameterModifier[] { mod };
            }

            MethodInfo mif = type.GetMethod(name, flags, null, argTypes, modifiers);

            if (mif == null)
                throw new ArgumentException($"Function {name}({string.Join(", ", argTypes.Select(x => x.Name))}) not defined on {type.FullName}");

            if (mif.IsStatic == (instance == null))
                return Delegate.CreateDelegate(typeof(TFunc), instance, mif) as TFunc;
            else
                throw new ArgumentOutOfRangeException(nameof(instance), "Method instance not correct");
        }

        /// <summary>
        /// Get the method matching the specification
        /// </summary>
        /// <typeparam name="TFunc"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static TFunc GetMethodWithInstance<TFunc>(this Type type, string name, BindingFlags flags)
            where TFunc : Delegate
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            flags |= BindingFlags.Instance;

            MethodInfo invokeInfo = typeof(TFunc).GetMethod(nameof(Action.Invoke));
            ParameterInfo[] parameters = invokeInfo.GetParameters().Skip(1).ToArray();
            Type[] argTypes = parameters.Select(x => x.ParameterType).ToArray();

            ParameterModifier[] modifiers = null;

            if (parameters.Any(x => x.IsOut))
            {
                ParameterModifier mod = new ParameterModifier(parameters.Length);
                for (int i = 0; i < parameters.Length; i++)
                {
                    mod[i] = parameters[i].IsOut;
                }
                modifiers = new ParameterModifier[] { mod };
            }

            MethodInfo mif = type.GetMethod(name, flags, null, argTypes, modifiers);

            if (mif == null)
                throw new ArgumentException($"Function {name}({string.Join(", ", argTypes.Select(x => x.Name))}) not defined on {type.FullName}");

            ParameterExpression[] exprParams = invokeInfo.GetParameters().Select(x => Expression.Parameter(x.ParameterType, x.Name)).ToArray();

            return Expression.Lambda<TFunc>(Expression.Call(exprParams[0], mif, exprParams.Skip(1).ToArray()), exprParams).Compile();
        }

        /// <summary>
        /// Gets the constructor matching the specific specification
        /// </summary>
        /// <typeparam name="TFunc"></typeparam>
        /// <param name="type"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static TFunc GetConstructor<TFunc>(this Type type, BindingFlags flags)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            flags |= BindingFlags.Instance;

            MethodInfo invokeInfo = typeof(TFunc).GetMethod(nameof(Action.Invoke));

            ParameterInfo[] parameters = invokeInfo.GetParameters();
            Type[] argTypes = parameters.Select(x => x.ParameterType).ToArray();

            ParameterModifier[] modifiers = null;

            if (parameters.Any(x => x.IsOut))
            {
                ParameterModifier mod = new ParameterModifier(parameters.Length);
                for (int i = 0; i < parameters.Length; i++)
                {
                    mod[i] = parameters[i].IsOut;
                }
                modifiers = new ParameterModifier[] { mod };
            }

            ConstructorInfo cif = type.GetConstructor(flags, null, argTypes, modifiers);

            if (cif == null)
                throw new ArgumentException($"Constructor {type.FullName}({string.Join(", ", argTypes.Select(x => x.Name))}) not found");

            ParameterExpression[] exprParams = parameters.Select(x => Expression.Parameter(x.ParameterType, x.Name)).ToArray();

            return Expression.Lambda<TFunc>(Expression.New(cif, exprParams), exprParams).Compile();
        }


    }
}
