using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TerminalRbkm.Infrastructure
{
    public class TabModel
    {
        public string Header { get; set; }
        public UserControl Content { get; set; }

        public TabModel(UserControl content, string header)
        {
            Content = content;
            Header = header;
        }
    }
}
