using System.Collections.Generic;

namespace VertMagazineSolution.Model
{
    public class Subscriber
    {
        public string id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public List<int> magazineIds { get; set; }
    }
}
