using System;
using VertMagazineSolution.Service;

namespace VertMagazineSolution
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            try
            {
                VertMagazineService service = new VertMagazineService();

                //for loop for demostrating multiple run time
                for (var i = 0; i < 5; i++) {
                    var res = await service.GetResults();
                    Console.WriteLine($"Result got success with timming {res.totalTime}");
                }
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
