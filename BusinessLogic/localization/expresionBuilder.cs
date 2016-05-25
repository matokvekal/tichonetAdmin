namespace Business_Logic
{


    public class DictExpressionBuilderSystem
    {
        // private static readonly ILog logger = LogManager.GetLogger(typeof(DictExpressionBuilderSystem));
        public static string Translate(string expression)
        {
            return localizedSystemDisplayNameAttribute.GetMessageFromResource(expression);
        }
    }
}


