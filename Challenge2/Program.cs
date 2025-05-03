namespace Challenge2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] arr = new int[10];
            arr[0] = 100;
            arr[^1] = 1000;

            //arr[9] = 1000;
            //arr[arr.Length - 1] = 1000;

            Console.WriteLine(arr[0]);
            Console.WriteLine(arr[^1]);
            Console.WriteLine(arr[4]);
        }
    }
}
