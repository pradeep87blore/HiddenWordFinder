using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordFinder
{
    public class FoundWordAttributes
    {
        public string word ="";
        public int startRow = -1, endRow = -1;
        public int startColumn = -1, endColumn = -1; 
        
        public FoundWordAttributes(string foundWord, int sRow, int sCol, int eRow, int eCol)
        {
            word = foundWord;
            startRow = sRow;
            startColumn = sCol;
            endRow = eRow;
            endColumn = eCol;
        }
    }
}
