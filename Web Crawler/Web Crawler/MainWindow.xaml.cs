using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Web_Crawler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var db = new dbContainer();
            var entities = db.Entity1;
            var someEntities = entities.Where(x => x.Id > 4 && isValidId(x.Id));
            var list = getList();
            var someIds = list.Where(x => x > 4 && isValidId(x));
        }

        public bool isValidId(int id)
        {
            return id % 3 != 2;
        }
        public IList<int> getList()
        {
            return new List<int>() { 5, 1, 123, 510, 42, 124 };
        }
    }
}
