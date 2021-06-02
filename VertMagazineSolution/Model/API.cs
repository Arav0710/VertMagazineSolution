namespace VertMagazineSolution.Model
{
    public static class API
    {
        public static string Token => "http://magazinestore.azurewebsites.net/api/token";
        public static string Categories => "http://magazinestore.azurewebsites.net/api/categories/#token#";
        public static string Magazines => "http://magazinestore.azurewebsites.net/api/magazines/#token#/";
        public static string Subscriber => "http://magazinestore.azurewebsites.net/api/subscribers/#token#/";
        public static string Answers => "http://magazinestore.azurewebsites.net/api/answer/#token#";
    }
}
