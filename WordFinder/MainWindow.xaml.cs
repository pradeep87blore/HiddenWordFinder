using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WordFinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> enteredWords;

        AllSentences allSentences = new AllSentences(); // Stores all sentences (l to r, r to l, t to b and b to t)

        public MainWindow()
        {
            InitializeComponent();

            InitEverything();
        }

        private void InitEverything()
        {
            enteredWords = new List<string>();
        }

        private void textBox_wordInput_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                var word = textBox_wordInput.Text;
                listBox_wordList.Items.Add(word.ToLower()); /// store all lower case letters only
                textBox_wordInput.Text = "";

                enteredWords.Add(word.ToLower());
            }
        }

        private void button_StartSearch_Click(object sender, RoutedEventArgs e)
        {
            CreateAllSentences();
        }

        private void CreateAllSentences()
        {
            foreach(string word in enteredWords)
            {
                allSentences.leftToRight.Add(word);

                string revWord = "";
                for (int iIndex = word.Length - 1; iIndex >= 0 ; iIndex--) // To create the right to left sentences
                {
                    revWord += word[iIndex];
                }

                allSentences.rightToLeft.Add(revWord);
            }

            int wordLength = enteredWords[0].Length; // Assuming all words have same length
            for(int iIndex = 0; iIndex < wordLength; iIndex++)
            {
                string topToBottomStr = "", bottomToTopStr = "";
                for(int iInner = 0; iInner < enteredWords.Count; iInner++)
                {
                    topToBottomStr += enteredWords[iInner][iIndex];
                    bottomToTopStr += enteredWords[enteredWords.Count - iInner - 1][iIndex];
                }

                allSentences.topToBottom.Add(topToBottomStr);
                allSentences.bottomToTop.Add(bottomToTopStr);
            }

            allSentences.PrintAllSentences();
        }
    }
}
