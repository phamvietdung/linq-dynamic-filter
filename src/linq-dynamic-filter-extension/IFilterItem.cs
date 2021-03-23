using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace linq_dynamic_filter_extension
{
    //public enum ECompareType
    //{
    //    equal = 0, // string only
    //    greaterThan = 1,
    //    greaterThanAndEqual = 2,
    //    lessThan = 3,
    //    lessThanAndEqual = 4,
    //    include = 5 // string only
    //}

    //public enum FieldType
    //{
    //    number = 0,
    //    text = 1,
    //    date = 2
    //}

    public static class CompareTypeConst
    {
        public static string equal = "equal";
        public static string notEqual = "notEqual";
        public static string greaterThan = "greaterThan";
        public static string greaterThanOrEqual = "greaterThanOrEqual";
        public static string lessThan = "lessThan";
        public static string lessThanOrEqual = "lessThanOrEqual";
        public static string include = "include";
        public static string notInclude = "notInclude";

        public static string fromTo = "fromTo";
        public static string fromToWithEqual = "fromToWithEqual";

    }

    public static class FieldTypeConst
    {
        public static string integerNumber = "integer";
        public static string decimalNumber = "decimal";
        public static string text = "text";
        public static string date = "date";

    }

    public interface IHasTypeProperty
    {
        public String Type { get; set; }
    }

    public interface IHasKeyProperty
    {
        public String Key { get; set; }
    }

    public interface IHasOperatorProperty
    {
        public String Operator { get; set; }
    }

    public interface IHasValueProperty
    { 
        public String Value { get; set; }
    }

    /// <summary>
    /// Property is required to using range filter
    /// </summary>
    public interface IHasStartProperty
    {
        public String Start { get; set; }
    }

    /// <summary>
    /// Property is required to using range filter
    /// </summary>
    public interface IHasEndProperty
    {
        public String End { get; set; }
    }

    /// <summary>
    /// Property required when filter batch value, just support equal or include operator
    /// </summary>
    public interface IHasBatchValueProperty
    {
        public List<string> Values { get; set; }
    }
}
