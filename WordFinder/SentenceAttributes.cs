using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordFinder
{
    public class SentenceAttributes
    {
        public enum Direction { LeftToRight, RightToLeft, TopToBottom, BottomToTop};

        public List<string> sentence;

        public Direction direction;

        public SentenceAttributes(List<string> sentence, Direction sDirection)
        {
            this.sentence = sentence;
            direction = sDirection;
        }
    }
}
