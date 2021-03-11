using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace linq_dynamic_filter_extension
{
    public class DFilterExtension<TEntity, TFilter> 
        where TFilter : IHasKeyProperty, IHasOperatorProperty, IHasValueProperty, IHasTypeProperty, IHasStartProperty, IHasEndProperty
    {

        public DFilterExtension() { }

        public List<String> CompareType = new List<String>() {

            CompareTypeConst.equal, 
            CompareTypeConst.notEqual,

            CompareTypeConst.greaterThan, 
            CompareTypeConst.greaterThanOrEqual, 

            CompareTypeConst.include, 
            CompareTypeConst.notInclude,

            CompareTypeConst.lessThan, 
            CompareTypeConst.lessThanOrEqual,

            CompareTypeConst.fromTo,
            CompareTypeConst.fromToWithEqual
        };

        public List<String> FieldType = new List<String>() { 
            FieldTypeConst.text,
            FieldTypeConst.date,
            FieldTypeConst.integerNumber,
            FieldTypeConst.decimalNumber
        };

        public Boolean ValidatedFilter(List<TFilter> filter)
        {
            Boolean valid = true;

            foreach (var f in filter)
            {
                if (CompareType.IndexOf(f.Operator) < 0 || FieldType.IndexOf(f.Type) < 0)
                {
                    valid = false;

                    break;
                }

            }

            return valid;
        }

        public Boolean ValidatedFilter(String filter)
        {
            return ValidatedFilter(ConvertFromString(filter));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Sample
        ///     
        ///     [
        ///         {
        ///             "Type" : "string",
        ///             "Value" : "Name",
        ///             "Operator" : "=",
        ///             "Value" : "John doe"
        ///         },
        ///         {
        ///             "Type" : "number",
        ///             "Value" : "Age",
        ///             "Operator" : ">",
        ///             "Value" : "20"
        ///         }
        ///     ]
        ///     
        ///  String : [{"Type" : "string","Value" : "Name","Operator" : "=","Value" : "John doe"},{"Type" : "number","Value" : "Age","Operator" : ">","Value" : "20"}]
        /// </remarks>
        public List<TFilter> ConvertFromString(String filter)
        {
            try { return JsonSerializer.Deserialize<List<TFilter>>(filter); }
            catch(Exception ex) { throw new Exception(ex.Message); }
        }

        public IQueryable<TEntity> Filterable(IQueryable<TEntity> source, String filter)
        {
            return Filterable(source, ConvertFromString(filter));
        }

        #region Handle

        private IQueryable<TEntity> HandleText(IQueryable<TEntity> source, TFilter filter)
        {
            if(filter.Operator == CompareTypeConst.equal || filter.Operator == CompareTypeConst.notEqual)
            {
                //ParameterExpression pe = Expression.Parameter(typeof(TEntity), "w");

                //Expression left = Expression.Property(pe, CapitalizeFirstLetter(filter.Key));

                //Expression right = Expression.Constant(filter.Value);

                //Expression e1 = filter.Operator == CompareTypeConst.equal 
                //    ? Expression.Equal(left, right) 
                //    : Expression.NotEqual(left, right);

                //MethodCallExpression whereCallExpression = Expression.Call(typeof(Queryable),
                //    "Where",
                //    new Type[] { source.ElementType },
                //    source.Expression,
                //    Expression.Lambda<Func<TEntity, bool>>(e1, new ParameterExpression[] { pe }));

                //return source.Provider.CreateQuery<TEntity>(whereCallExpression);

                return EqualAndNotEqualHandle(source, filter);
            }

            if(filter.Operator == CompareTypeConst.include || filter.Operator == CompareTypeConst.notInclude)
            {
                ParameterExpression pe = Expression.Parameter(typeof(TEntity), "i");

                Expression left = Expression.Property(pe, CapitalizeFirstLetter(filter.Key));

                Expression right = Expression.Constant(filter.Value);

                MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });

                var containsCallExpression = Expression.Call(left, method, right);

                var positiveCall = Expression.Lambda<Func<TEntity, bool>>(containsCallExpression, pe);

                var negativeCall = Expression.Lambda<Func<TEntity, bool>>(Expression.Not(containsCallExpression), pe);

                return source.Where(filter.Operator == CompareTypeConst.include ? positiveCall : negativeCall);

                //return source.Provider.CreateQuery<TEntity>(containsMethodExp);
            }

            throw Notsupport(filter);

        }
        
        private IQueryable<TEntity> HandleNumberOrDate(IQueryable<TEntity> source, TFilter filter)
        {
            if (filter.Operator == CompareTypeConst.equal || filter.Operator == CompareTypeConst.notEqual)
            {
                return EqualAndNotEqualHandle(source, filter);
            }

            if(filter.Operator == CompareTypeConst.greaterThan 
                || filter.Operator == CompareTypeConst.greaterThanOrEqual
                || filter.Operator == CompareTypeConst.lessThan
                || filter.Operator == CompareTypeConst.lessThanOrEqual
                )
            {
                ParameterExpression pe = Expression.Parameter(typeof(TEntity), "w");

                Type compareType = GetTypeToCompare(filter);

                Expression left = Expression.Property(pe, CapitalizeFirstLetter(filter.Key));

                Expression right = Expression.Constant(Convert.ChangeType(filter.Value, compareType));

                Expression e1;

                if (filter.Operator == CompareTypeConst.greaterThan)
                    e1 = Expression.GreaterThan(left, right);
                else if (filter.Operator == CompareTypeConst.greaterThanOrEqual)
                    e1 = Expression.GreaterThanOrEqual(left, right);
                else if (filter.Operator == CompareTypeConst.lessThan)
                    e1 = Expression.LessThan(left, right);
                else
                    e1 = Expression.LessThanOrEqual(left, right);

                MethodCallExpression whereCallExpression = Expression.Call(typeof(Queryable),
                    "Where",
                    new Type[] { source.ElementType },
                    source.Expression,
                    Expression.Lambda<Func<TEntity, bool>>(e1, new ParameterExpression[] { pe }));

                return source.Provider.CreateQuery<TEntity>(whereCallExpression);
            }

            if(filter.Operator == CompareTypeConst.fromTo || filter.Operator == CompareTypeConst.fromToWithEqual)
            {

                ParameterExpression pe = Expression.Parameter(typeof(TEntity), "w");

                Type compareType = GetTypeToCompare(filter);

                #region start

                Expression start_left = Expression.Property(pe, CapitalizeFirstLetter(filter.Key));

                Expression start_right = Expression.Constant(Convert.ChangeType(filter.Start, compareType));

                Expression start_e1;

                if (filter.Operator == CompareTypeConst.fromTo)
                    start_e1 = Expression.GreaterThan(start_left, start_right);
                else
                    start_e1 = Expression.GreaterThanOrEqual(start_left, start_right);

                MethodCallExpression startWhereCallExpression = Expression.Call(typeof(Queryable),
                    "Where",
                    new Type[] { source.ElementType },
                    source.Expression,
                    Expression.Lambda<Func<TEntity, bool>>(start_e1, new ParameterExpression[] { pe }));

                source = source.Provider.CreateQuery<TEntity>(startWhereCallExpression);

                #endregion

                #region end

                Expression end_left = Expression.Property(pe, CapitalizeFirstLetter(filter.Key));

                Expression end_right = Expression.Constant(Convert.ChangeType(filter.End, compareType));

                Expression end_e1;

                if (filter.Operator == CompareTypeConst.fromTo)
                    end_e1 = Expression.LessThan(end_left, end_right);
                else
                    end_e1 = Expression.LessThanOrEqual(end_left, end_right);

                MethodCallExpression endWhereCallExpression = Expression.Call(typeof(Queryable),
                    "Where",
                    new Type[] { source.ElementType },
                    source.Expression,
                    Expression.Lambda<Func<TEntity, bool>>(end_e1, new ParameterExpression[] { pe }));

                source = source.Provider.CreateQuery<TEntity>(endWhereCallExpression);

                #endregion

                return source;


            }

            throw Notsupport(filter);
        }

        #endregion

        #region Common 

        private IQueryable<TEntity> EqualAndNotEqualHandle(IQueryable<TEntity> source, TFilter filter
            )
        {
            ParameterExpression pe = Expression.Parameter(typeof(TEntity), "w");

            Type compareType = GetTypeToCompare(filter);

            Expression left = Expression.Property(pe, CapitalizeFirstLetter(filter.Key));

            Expression right = Expression.Constant(Convert.ChangeType(filter.Value, compareType));

            Expression e1 = filter.Operator == CompareTypeConst.equal
                ? Expression.Equal(left, right)
                : Expression.NotEqual(left, right);

            MethodCallExpression whereCallExpression = Expression.Call(typeof(Queryable),
                "Where",
                new Type[] { source.ElementType },
                source.Expression,
                Expression.Lambda<Func<TEntity, bool>>(e1, new ParameterExpression[] { pe }));

            return source.Provider.CreateQuery<TEntity>(whereCallExpression);
        }

        private Exception Notsupport(TFilter filter)
        {
            return new Exception(String.Format(@"Currently, we aren't support type [{0}] and [{1}] operator", filter.Type, filter.Operator));
        }

        private Type GetTypeToCompare(TFilter filter)
        {
            if (filter.Type == FieldTypeConst.text)
                return typeof(string);
            else if (filter.Type == FieldTypeConst.integerNumber)
                return typeof(int);
            else if (filter.Type == FieldTypeConst.decimalNumber)
                return typeof(decimal);
            else if (filter.Type == FieldTypeConst.date)
                return typeof(DateTime);

            throw new Exception("Field Type Not Found!");
        }

        private string CapitalizeFirstLetter(string input)
        {
            string str = input;

            if (str.Length == 0)
                throw new Exception("Empty String");
            else if (str.Length == 1)
                return char.ToUpper(str[0]).ToString();
            else
                return (char.ToUpper(str[0]) + str.Substring(1)).ToString();
        }

        #endregion

        public IQueryable<TEntity> Filterable(IQueryable<TEntity> source, List<TFilter> filter)
        {

            if (!ValidatedFilter(filter))
                throw new Exception("Filter Format Incorrected!");

            if (filter.Count > 0)
            {

                for (var i = 0; i < filter.Count(); i++)
                {
                    var current_filter = filter[i];

                    if(current_filter.Type == FieldTypeConst.text)
                        source = HandleText(source, current_filter);

                    if (current_filter.Type == FieldTypeConst.integerNumber || current_filter.Type == FieldTypeConst.decimalNumber)
                        source = HandleNumberOrDate(source, current_filter);

                    if(current_filter.Type == FieldTypeConst.date)
                        source = HandleNumberOrDate(source, current_filter);

                }
            }

            return source;
        }

       
    }
}
