using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordFinder
{
    public class AllSentences
    {
        public List<string> leftToRight;
        public List<string> rightToLeft;
        public List<string> topToBottom;
        public List<string> bottomToTop;

        public AllSentences()
        {
            leftToRight = new List<string>();
            rightToLeft = new List<string>();
            topToBottom = new List<string>();
            bottomToTop = new List<string>();
        }

        public void PrintAllSentences()
        {
            Console.WriteLine("\nPrinting left to right words");

            foreach(string word in leftToRight)
            {
                Console.WriteLine(word);
            }

            Console.WriteLine("\nPrinting right to left words");

            foreach (string word in rightToLeft)
            {
                Console.WriteLine(word);
            }

            Console.WriteLine("\nPrinting top to bottom words");

            foreach (string word in topToBottom)
            {
                Console.WriteLine(word);
            }

            Console.WriteLine("\nPrinting bottom to top words");

            foreach (string word in bottomToTop)
            {
                Console.WriteLine(word);
            }
        }
    }
}
