using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

        HashSet<string> allEnglishWords = new HashSet<string>();

        Thread readerThread;

        static int searchThreadCounter = 0;
        List<FoundWordAttributes> foundWords = new List<FoundWordAttributes>();

        Semaphore sem = new Semaphore(0, 1);

        int minWordLength = 3;

        string sChosenFile = "";

        private const string ENTER_WORDS = "Enter the word here and press enter:";
        private const string CHOOSE_FILE = "The following is the file you have chosen. Press enter to continue:";

        public MainWindow()
        {
            InitializeComponent();

            InitEverything();

            readerThread.Start();
        }

        private void InitEverything()
        {
            enteredWords = new List<string>();

            readerThread = new Thread(ReadAllWords);

            radioButton_enterText.IsChecked = true;

        }

        // This function reads all the words in english from the text file at C:\Words.txt
        // TODO: Need to make this more dynamic
        private void ReadAllWords()
        {
            string[] engWordCollection = File.ReadAllLines("C:\\Words.txt");

            foreach(string word in engWordCollection)
            {
                allEnglishWords.Add(word.ToLower());
            }
        }

        private void textBox_wordInput_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                if (radioButton_enterText.IsChecked == true)
                {
                    var word = textBox_wordInput.Text;
                    listBox_wordList.Items.Add(word.ToLower()); /// store all lower case letters only
                    textBox_wordInput.Text = "";

                    enteredWords.Add(word.ToLower());
                }
                else
                {
                    var words = File.ReadAllLines(sChosenFile);
                    foreach (var word in words)
                    {
                        listBox_wordList.Items.Add(word.ToLower()); /// store all lower case letters only
                        enteredWords.Add(word.ToLower());
                    }
                }
            }
        }

        private void button_StartSearch_Click(object sender, RoutedEventArgs e)
        {
            // If the thread reading the english words is still not yet done, wait for it to complete
            while(readerThread.IsAlive == true)
            {
                Thread.Sleep(100);
            }

            // Create all the sentences (left to right, right to left, top to bottom and bottom to top)
            CreateAllSentences();

            minWordLength = Int32.Parse(textBox_minWordLength.Text);
            // Start the search operation
            StartSearching();
        }

        private void StartSearching()
        {
            Thread leftToRightThread = new Thread(new ParameterizedThreadStart(SearchForWords));
            Thread rightToLeftThread = new Thread(new ParameterizedThreadStart(SearchForWords));
            Thread topToBottomThread = new Thread(new ParameterizedThreadStart(SearchForWords));
            Thread bottomToTopThread = new Thread(new ParameterizedThreadStart(SearchForWords));

            leftToRightThread.Start(new SentenceAttributes(allSentences.leftToRight, SentenceAttributes.Direction.LeftToRight));
            searchThreadCounter++;
            rightToLeftThread.Start(new SentenceAttributes(allSentences.rightToLeft, SentenceAttributes.Direction.RightToLeft));
            searchThreadCounter++;
            topToBottomThread.Start(new SentenceAttributes(allSentences.topToBottom, SentenceAttributes.Direction.TopToBottom));
            searchThreadCounter++;
            bottomToTopThread.Start(new SentenceAttributes(allSentences.bottomToTop, SentenceAttributes.Direction.BottomToTop));
            searchThreadCounter++;

            while (searchThreadCounter != 0)
            {
                Thread.Sleep(100); // Sleep this thread for 100ms and check again
            }

            DoneWithSearch();
        }

        Action onCompleted = () =>
        {
            searchThreadCounter--;
        };

        private void DoneWithSearch()
        {
            foreach (var found in foundWords)
            {
                if (found == null)
                    continue;
                listBox_output.Items.Add(string.Format("{0} found starting from row {1}, col {2} to row {3}, col {4}", found.word, 
                    found.startRow, found.startColumn,
                    found.endRow, found.endColumn));
            }
        }

        void SearchForWords(object obj)
        {
            try
            {
                SentenceAttributes sAttr = (SentenceAttributes)obj;
                List<string> listOfSentences = (List<string>)sAttr.sentence;


                Console.WriteLine(string.Format("-----{0}------", sAttr.direction));
                // List of sentences act as the columns, the words in each represent the rows
                for (int iSentences = 0; iSentences < listOfSentences.Count; iSentences++)
                {
                    // Iterate over the sentence. Start with the largest word, decrement the word size from the right as you go and thus reach till the end
                    // Next, start from the second letter and proceed till the end. Continue till the first and last letters of the word are the last letter
                    //Console.WriteLine(sentence);

                    string sentence = listOfSentences[iSentences];
                    for (int iStartLetter = 0; iStartLetter < sentence.Length; iStartLetter++)
                    {
                        for (int iLastLetter = sentence.Length; iLastLetter >= iStartLetter; iLastLetter--)
                        {
                            if ((iLastLetter - iStartLetter) < minWordLength)
                                continue; // Dont search for words smaller than the specified minimum size

                            string tempString = sentence.Substring(iStartLetter, iLastLetter - iStartLetter);

                            if (IsItAWord(tempString)) // Check if this is a word or not
                            {
                                int iStartRow = iSentences, iStartCol = iStartLetter, iEndRow = 0, iEndCol = 0;

                                if (sAttr.direction == SentenceAttributes.Direction.LeftToRight)
                                {
                                    iEndRow = iStartRow; // Since this is a horizontal scan, the rows will be same.
                                    // Column calc: E.g., if the word FUTURE starts at column 2 (i.e., 3rd column), the end will be (length = 6) + 2 - 1 = 7 (-1 because col. 2 is already used)
                                    iEndCol = tempString.Length + iStartLetter - 1; 
                                }
                                else if (sAttr.direction == SentenceAttributes.Direction.RightToLeft)
                                {
                                    iEndRow = iStartRow; // Same as the start row since this is a horizonatal scan
                                    iEndCol = iStartLetter - tempString.Length + 1; 
                                }
                                else if (sAttr.direction == SentenceAttributes.Direction.TopToBottom)
                                {
                                    
                                }
                                else
                                {

                                }
                                //Console.WriteLine(tempString);
                                AddFoundString(new FoundWordAttributes(tempString, iStartRow, iStartCol, iEndRow, iEndCol));
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            finally
            {
                Console.WriteLine("-------------");

                onCompleted();
            }
        }

        

        private void AddFoundString(FoundWordAttributes foundWord)
        {
            //sem.WaitOne();

            foundWords.Add(foundWord);

            //sem.Release();
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

            // allSentences.PrintAllSentences(); // Debug
        }


        bool IsItAWord(string str)
        {
            //Console.WriteLine(str);
            if (allEnglishWords.Contains(str))
                return true;

            return false;
        }
               

        private void radioButton_enterText_Checked(object sender, RoutedEventArgs e)
        {
            radioButton_ChooseFile.IsChecked = false;
            label_enterTextOrChooseFile.Content = ENTER_WORDS;
        }

        private void radioButton_ChooseFile_Checked(object sender, RoutedEventArgs e)
        {
            radioButton_enterText.IsChecked = false;
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
                textBox_wordInput.Text = openFileDialog.FileName;

            label_enterTextOrChooseFile.Content = CHOOSE_FILE;

            sChosenFile = openFileDialog.FileName;
        }

        private void button_CopyResToClipboard_Click(object sender, RoutedEventArgs e)
        {
            string allResults = "";

            foreach (var item in listBox_output.Items)
            {
                if (allResults != null)
                    allResults += "\n";

                allResults += item.ToString();
            }

            if (allResults != null)
                Clipboard.SetText(allResults);
        }
    }
}
