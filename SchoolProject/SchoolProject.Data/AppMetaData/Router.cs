using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProject.Data.AppMetaData
{
    public static class Router
    {
        public const string Root = "Api";
        public const string Version = "V1";
        public const string Rule = $"{Root}/{Version}";

        public static class StudentRouting
        {
            public const string Prefix = $"/{Rule}/Student";
            public const string List = $"{Prefix}/List";
            public const string GetByID = Prefix + "/" + "{id}";
            public const string Create = Prefix + "/" + "Create";
            public const string Update = Prefix + "/" + "Update";
            public const string Delete = Prefix + "/" + "Delete{StudID}";
            public const string Paginated = Prefix + "/" + "Paginated";
        }

    }
}
