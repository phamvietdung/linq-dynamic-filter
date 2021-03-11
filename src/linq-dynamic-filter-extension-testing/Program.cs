using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using linq_dynamic_filter_extension;

namespace linq_dynamic_filter_extension_testing
{
    public class Program
    {

        public static void Main(string[] args)
        {
            String file_path = AppDomain.CurrentDomain.BaseDirectory + "data.json";

            if (!File.Exists(file_path))
            {
                throw new Exception("data.json not exists!");
            }

            String dataSource = "";//= JsonSerializer.Serialize<List<Person>>(new List<Person>(){});

            using (StreamReader sr = File.OpenText(file_path))
            {
                string l;
                while ((l = sr.ReadLine()) != null)
                {
                    dataSource += l.Trim();
                }
                
            }

            /// <summary>
            /// String filter_equal = "[{\"Type\" : \"text\",\"Key\" : \"Name\",\"Operator\" : \"equal\",\"Value\" : \"Woodard Coffey\"}]";
            /// </summary>

            #region filter text

            String filter_equal = JsonSerializer.Serialize<List<Filter>>(new List<Filter>() {
                new Filter()
                {
                    Type = FieldTypeConst.text,
                    Key = "Name",
                    Operator = CompareTypeConst.equal,
                    Value = "Mann Newton"
                }
            });

            String filter_not_equal = JsonSerializer.Serialize<List<Filter>>(new List<Filter>() {
                new Filter()
                {
                    Type = FieldTypeConst.text,
                    Key = "Name",
                    Operator = CompareTypeConst.notEqual,
                    Value = "Mann Newton"
                }
            });

            String filter_include = "[{\"Type\" : \"text\",\"Key\" : \"Name\",\"Operator\" : \"include\",\"Value\" : \"a\"}]";

            String filter_not_include = JsonSerializer.Serialize<List<Filter>>(new List<Filter>()
            {
                new Filter()
                {
                    Type = FieldTypeConst.text,
                    Operator = CompareTypeConst.notInclude,
                    Key = "Name",
                    Value = "a"
                }
            });

            String filter_not_support_case = "[{\"Type\" : \"text\",\"Key\" : \"Name\",\"Operator\" : \"lessThan\",\"Value\" : \"a\"}]";

            #endregion

            #region filter number

            String number_filter_not_equal = JsonSerializer.Serialize<List<Filter>>(new List<Filter>()
            {
                new Filter()
                {
                    Type = FieldTypeConst.integerNumber,
                    Operator = CompareTypeConst.equal,
                    Key = "Age",
                    Value = "49"
                },
                new Filter()
                {
                    Type = FieldTypeConst.integerNumber,
                    Operator = CompareTypeConst.greaterThanOrEqual,
                    Key = "Age",
                    Value = "49"
                }
            });

            String number_filter_range = JsonSerializer.Serialize<List<Filter>>(new List<Filter>()
            {
                new Filter()
                {
                    Type = FieldTypeConst.integerNumber,
                    Operator = CompareTypeConst.greaterThanOrEqual,
                    Key = "Age",
                    Value = "36"
                },
                new Filter()
                {
                    Type = FieldTypeConst.integerNumber,
                    Operator = CompareTypeConst.lessThanOrEqual,
                    Key = "Age",
                    Value = "55"
                }
            });

            string number_decimal_filter_equal = JsonSerializer.Serialize<List<Filter>>(new List<Filter>()
            {
                new Filter()
                {
                    Type = FieldTypeConst.decimalNumber,
                    Operator = CompareTypeConst.equal,
                    Key = "Amount",
                    Value = "81236.1418736223"
                },
                new Filter()
                {
                    Type = FieldTypeConst.decimalNumber,
                    Operator = CompareTypeConst.notEqual,
                    Key = "Amount",
                    Value = "427368.6921921577"
                }
            });

            string number_decimal_filter_range = JsonSerializer.Serialize<List<Filter>>(new List<Filter>()
            {
                new Filter()
                {
                    Type = FieldTypeConst.decimalNumber,
                    Operator = CompareTypeConst.greaterThanOrEqual,
                    Key = "Amount",
                    Value = "271421.5981646036"
                },
                new Filter()
                {
                    Type = FieldTypeConst.decimalNumber,
                    Operator = CompareTypeConst.lessThanOrEqual,
                    Key = "Amount",
                    Value = "729774.7005211266"
                }
            });

            string number_decimal_filter_range2 = JsonSerializer.Serialize<List<Filter>>(new List<Filter>()
            {
                new Filter()
                {
                    Type = FieldTypeConst.decimalNumber,
                    Operator = CompareTypeConst.fromTo,
                    Key = "Amount",
                    Start = "419538.2190820557",
                    End ="988957.9193887961"
                }
            });

            string number_decimal_filter_range_with_equal = JsonSerializer.Serialize<List<Filter>>(new List<Filter>()
            {
                new Filter()
                {
                    Type = FieldTypeConst.decimalNumber,
                    Operator = CompareTypeConst.fromToWithEqual,
                    Key = "Amount",
                    Start = "419538.2190820557",
                    End ="988957.9193887961"
                }
            });

            #endregion

            #region datetime

            String datetime_filter_equal = JsonSerializer.Serialize<List<Filter>>(new List<Filter>()
            {
                new Filter()
                {
                    Type = FieldTypeConst.date,
                    Operator = CompareTypeConst.equal,
                    Key = "DOB",
                    Value = "2001-04-15T20:03:37+00:00" //new DateTime(2001,4,15,20,3,37).ToString()
                }
            });

            String datetime_filter_range = JsonSerializer.Serialize<List<Filter>>(new List<Filter>()
            {
                new Filter()
                {
                    Type = FieldTypeConst.date,
                    Operator = CompareTypeConst.greaterThan,
                    Key = "DOB",
                    Value = "2001-04-15T20:03:37+00:00" //new DateTime(2001,4,15,20,3,37).ToString()
                },
                new Filter()
                {
                    Type = FieldTypeConst.date,
                    Operator = CompareTypeConst.lessThanOrEqual,
                    Key = "DOB",
                    Value = "2017-09-09T04:41:07+00:00" //new DateTime(2001,4,15,20,3,37).ToString()
                }
            });

            string datetime_filter_range2 = JsonSerializer.Serialize<List<Filter>>(new List<Filter>()
            {
                new Filter()
                {
                    Type = FieldTypeConst.date,
                    Operator = CompareTypeConst.fromTo,
                    Key = "DOB",
                    Start = "1993-08-22T08:30:42+00:00",
                    End ="2006-02-27T02:12:58+00:00"
                }
            });

            string datetime_filter_range_with_equal = JsonSerializer.Serialize<List<Filter>>(new List<Filter>()
            {
                new Filter()
                {
                    Type = FieldTypeConst.date,
                    Operator = CompareTypeConst.fromToWithEqual,
                    Key = "DOB",
                    Start = "1993-08-22T08:30:42+00:00",
                    End ="2006-02-27T02:12:58+00:00"
                }
            });

            #endregion

            IQueryable<Person> source = JsonSerializer.Deserialize<List<Person>>(dataSource).AsQueryable();

            Console.WriteLine("Start Testing!");

            Console.WriteLine(String.Format(@"DataSource : {0} item", source.Count()));

            var ins = new DFilterExtension<Person, Filter>();

            Console.WriteLine("-------note that currently im going testing with 1 filter each-------");

            BreakSomeLine("Testing text");

            foreach (var f in new List<string>(){
                filter_equal, filter_not_equal,
                filter_include, filter_not_include,
                filter_not_support_case,
            })
            {
                TestCase(ins, source, f);
            };

            BreakSomeLine("Testing number");

            foreach (var f in new List<string>(){
                number_filter_not_equal,
                number_filter_range,
                number_decimal_filter_equal,
                number_decimal_filter_range,
                number_decimal_filter_range2,
                number_decimal_filter_range_with_equal
            })
            {
                TestCase(ins, source, f);
            };

            BreakSomeLine("Testing datetime");

            foreach (var f in new List<string>(){
                datetime_filter_equal,
                datetime_filter_range,
                datetime_filter_range2,
                datetime_filter_range_with_equal
            })
            {
                TestCase(ins, source, f);
            };

            Console.ReadKey();
        }

        private static void TestCase(DFilterExtension<Person, Filter> ins, IQueryable<Person> source, string filters)
        {
            try
            {
                var f = JsonSerializer.Deserialize<List<Filter>>(filters).AsQueryable();

                Console.WriteLine(string.Format(@"Filter detail : Type = [{0}] Operator = [{1}]", f.FirstOrDefault().Type, String.Join(" and ", f.Select(s => s.Operator))));

                var rs = ins.Filterable(source, filters);

                Console.WriteLine(String.Format(@"Found : {0} result", rs.Count()));

            }catch(Exception ex)
            {
                Console.WriteLine(String.Format(@"Error case : {0} with error : {1}", filters, ex.Message));
            }
        }

        private static void BreakSomeLine(string broadcast_message = "") {
            for(var i =0; i < 5; i++)
            {
                Console.WriteLine(" ");
                
            }
            if (broadcast_message != null || broadcast_message != "")
                Console.WriteLine(String.Format(@"----------{0}----------", broadcast_message));
        }
    }

    public class Filter : IHasKeyProperty, IHasOperatorProperty, IHasValueProperty, IHasTypeProperty, IHasStartProperty, IHasEndProperty
    {
        public String Type { get; set; }
        public string Key { get; set; }
        public String Operator { get; set; }
        public string Value { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public Decimal Amount { get; set; }
        public DateTime DOB { get; set; }

    }
}
