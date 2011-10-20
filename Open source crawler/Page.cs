using System;

namespace CSharpCrawler
{
    public class Page
    {
        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Page() { }

        #endregion
        #region Private Instance Fields

        private int _size;
        private string _text;
        private string _url;
        private int _viewstateSize;

        #endregion
        #region Public Properties

        public int Size
        {
            get { return _size; }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                _size = value.Length;
            }
        }

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public int ViewstateSize
        {
            get { return _viewstateSize; }
            set { _viewstateSize = value; }
        }

        #endregion

        public void CalculateViewstateSize()
        {
            int startingIndex = Text.IndexOf("id=\"__VIEWSTATE\"");
            if (startingIndex > -1)
            {
                int indexOfViewstateValueNode = Text.IndexOf("value=\"", startingIndex);
                int indexOfClosingQuotationMark = Text.IndexOf("\"", indexOfViewstateValueNode + 7);
                string viewstateValue = Text.Substring(indexOfViewstateValueNode + 7, indexOfClosingQuotationMark - (indexOfViewstateValueNode + 7));

                ViewstateSize = viewstateValue.Length;
            }
        }
    }
}
