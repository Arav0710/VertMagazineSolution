using System.Collections.Generic;

namespace VertMagazineSolution.Model
{
    public class Results<T>
    {
        public List<T> data { get; set; }
        public string token { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
    }
    public class PostResults<T>
    {
        public T data { get; set; }
        public string token { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
    }
}
