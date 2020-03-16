using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Common.EF
{
    public static class WhereExpressions
    {
        #region 第一种方法，其实就是你现有方法的简写，实际没区别
        /// <summary>
        /// 如果条件成立，则给List添加Where子句
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="source">List</param>
        /// <param name="property">属性</param>
        /// <param name="IfFunc">是否给List添加Where子句</param>
        /// <param name="where">Where子句</param>
        /// <returns></returns>
        public static IQueryable<TSource> WhereIf<TSource, TProperty>(this IQueryable<TSource> source, TProperty property, Func<TProperty, bool> IfFunc, Expression<Func<TSource, bool>> where)

        {
            if (IfFunc(property) == true)
            {
                return source.Where(where);
            }
            else
            {
                return source;
            }
        }
        #endregion

        #region 第二种方法，需要自己实现的方法较多。用到反射
        /// <summary>
        /// 根据<paramref name="model"/>的所有公共属性，给List添加Where子句
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">List</param>
        /// <param name="model">过滤条件</param>
        /// <returns></returns>
        public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, TSource model)
        {
            var props = typeof(TSource).GetProperties();
            var ret = source;

            foreach (var prop in props)
            {
                if (prop.PropertyType == typeof(string)) //string类型的属性，判断是否加入过滤
                {
                    var propertyValue = (string)prop.GetValue(model);
                    Func<string, bool> ifFunc = p => string.IsNullOrEmpty(p) == false;
                    var where = GetEqualExpression<TSource>(model, prop);

                    ret = ret.WhereIf<TSource, string>(propertyValue, ifFunc, where);
                }
                else if (prop.PropertyType == typeof(int)) //int类型的属性，判断是否加入过滤
                {
                    var propertyValue = (int)prop.GetValue(model);
                    Func<int, bool> ifFunc = p => p > 0;
                    var where = GetEqualExpression<TSource>(model, prop);

                    ret = ret.WhereIf<TSource, int>(propertyValue, ifFunc, where);
                }
            }

            return ret;
        }

        /// <summary>
        /// 生成 A==B，表达式（其他<、>、>=自己按需求加）
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model">过滤条件对象</param>
        /// <param name="propertyInfo">过滤条件的某一个属性</param>
        /// <returns></returns>
        public static Expression<Func<TModel, bool>> GetEqualExpression<TModel>(TModel model, PropertyInfo propertyInfo)
        {
            var parameterName = "m";
            var expParameter = Expression.Parameter(typeof(TModel), parameterName);
            var expProperty = Expression.Property(expParameter, propertyInfo);
            var expConstant = Expression.Constant(propertyInfo.GetValue(model), propertyInfo.PropertyType);
            var expEqual = Expression.Equal(expProperty, expConstant);
            var expFunc = Expression.Lambda<Func<TModel, bool>>(expEqual, expParameter);
            return expFunc;
        }
        #endregion
    }
}
