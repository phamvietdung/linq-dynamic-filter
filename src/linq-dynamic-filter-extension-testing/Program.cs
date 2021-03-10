using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using linq_dynamic_filter_extension;

namespace linq_dynamic_filter_extension_testing
{
    public class Program
    {

        public static void Main(string[] args)
        {
            String dataSource = "[{\"Name\":\"Donna Perry\",\"Age\":49,\"DOB\":\"Wednesday, August 26, 2015 4:26 PM\",\"Address\":\"Ut ullamco in consequat mollit deserunt adipisicing. Ea ex reprehenderit ut do. Sunt irure consequat voluptate deserunt non tempor veniam labore. Ad ut laboris velit cupidatat laborum minim ut elit est proident pariatur sunt labore. Velit qui pariatur ipsum labore aliqua laboris culpa aliquip nostrud nostrud exercitation ullamco occaecat.\"},{\"Name\":\"Gilliam Cash\",\"Age\":90,\"DOB\":\"Sunday, September 24, 2017 12:23 AM\",\"Address\":\"Adipisicing cupidatat fugiat labore esse enim id sint quis laborum qui anim proident duis dolore. Proident cillum commodo proident irure quis do tempor nulla proident officia nisi ut. Mollit ex dolor duis cupidatat fugiat mollit amet do. Ea consectetur dolore nisi nulla reprehenderit enim ullamco magna aliquip officia adipisicing non. Officia ea elit id velit.\"},{\"Name\":\"Woodard Coffey\",\"Age\":71,\"DOB\":\"Tuesday, January 22, 2019 9:03 AM\",\"Address\":\"Ipsum labore occaecat aliqua eu sint proident ex elit veniam fugiat sit ex ad. In fugiat nisi mollit consequat do est. Laboris aliquip aute aliqua velit velit velit veniam irure laborum nulla irure. Voluptate id amet nisi commodo amet reprehenderit nisi esse nostrud aute ea ullamco.\"},{\"Name\":\"Lana Ramos\",\"Age\":76,\"DOB\":\"Friday, August 10, 2018 1:57 AM\",\"Address\":\"Voluptate Lorem nostrud magna fugiat deserunt duis officia cillum voluptate. Sunt dolore ad deserunt consectetur. Laborum officia irure sit non adipisicing qui ea irure eiusmod. Cupidatat ad anim adipisicing dolor id enim consequat nostrud in excepteur excepteur nulla. Nulla officia anim tempor excepteur quis ad deserunt sit.\"},{\"Name\":\"Becker Reid\",\"Age\":33,\"DOB\":\"Monday, March 30, 2015 12:27 AM\",\"Address\":\"Ipsum elit commodo tempor dolor pariatur deserunt laboris ad elit ex sunt id nisi non. Sunt id adipisicing officia irure nisi. Reprehenderit dolor enim ut et reprehenderit proident excepteur cupidatat sint cillum.\"}]";

            /// <summary>
            /// String filter_equal = "[{\"Type\" : \"text\",\"Key\" : \"Name\",\"Operator\" : \"equal\",\"Value\" : \"Woodard Coffey\"}]";
            /// </summary>
            String filter_equal = JsonSerializer.Serialize<List<Filter>>(new List<Filter>() {
                new Filter()
                {
                    Type = FieldTypeConst.text,
                    Key = "Name",
                    Operator = CompareTypeConst.equal,
                    Value = "Woodard Coffey"
                }
            });

            String filter_not_equal = JsonSerializer.Serialize<List<Filter>>(new List<Filter>() {
                new Filter()
                {
                    Type = FieldTypeConst.text,
                    Key = "Name",
                    Operator = CompareTypeConst.notEqual,
                    Value = "Woodard Coffey"
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

            IQueryable<Person> source = JsonSerializer.Deserialize<List<Person>>(dataSource).AsQueryable();

            Console.WriteLine("Start Testing!");

            Console.WriteLine(String.Format(@"DataSource : {0} item", source.Count()));

            var ins = new DFilterExtension<Person, Filter>();

            Console.WriteLine("-------note that currently im going testing with 1 filter each-------");

            foreach (var f in new List<string>(){
                filter_equal, filter_not_equal,
                filter_include, filter_not_include,
                filter_not_support_case,
            })
            {
                TestCase(ins, source, f);
            };

            //var equal = ins.Filterable(source, filter_equal);

            //Console.WriteLine(String.Format(@"Found : {0} result", equal.Count(), equal.FirstOrDefault().Age));

            //var inc = ins.Filterable(source, filter_include);

            //Console.WriteLine(String.Format(@"Found : {0} result", inc.Count(), inc.FirstOrDefault().Age));

            //var notInc = ins.Filterable(source, filter_not_include);

            //Console.WriteLine(String.Format(@"Found : {0} result", inc.Count(), inc.FirstOrDefault().Age));

            try
            {
                ins.Filterable(source, filter_not_support_case);
            }
            catch(Exception ex)
            {
                
            }


            Console.ReadKey();
        }

        private static void TestCase(DFilterExtension<Person, Filter> ins, IQueryable<Person> source, string filters)
        {
            try
            {
                var f = JsonSerializer.Deserialize<List<Filter>>(filters).AsQueryable();

                Console.WriteLine(string.Format(@"Filter detail : Type = [{0}] Operator = [{1}]", f.FirstOrDefault().Type, f.FirstOrDefault().Operator));

                var rs = ins.Filterable(source, filters);
                Console.WriteLine(String.Format(@"Found : {0} result", rs.Count()));
            }catch(Exception ex)
            {
                Console.WriteLine(String.Format(@"Error case : {0} with error : {1}", filters, ex.Message));
            }
        }
    }

    public class Filter : IHasKeyProperty, IHasOperatorProperty, IHasValueProperty, IHasTypeProperty
    {
        public String Type { get; set; }
        public string Key { get; set; }
        public String Operator { get; set; }
        public string Value { get; set; }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public String DOB { get; set; }
        public string Address { get; set; }
    }
}
