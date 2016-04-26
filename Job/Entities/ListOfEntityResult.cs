using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Create.CSP.GitHub.Reporting.Entities
{
    /// <summary>
    /// This class represents a model for returning a result containing a list of items
    /// and a continuation token to get the next page of items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListOfEntityResult<T> where T : class
    {
        /// <summary>
        /// The list of items.
        /// </summary>
        public List<T> Items { get; set; }

        /// <summary>
        /// Optional continuation token to get more items if existent.
        /// </summary>
        /// <remarks>This may not be used by all operations/entities. Some operations/entities
        /// just change the page for getting more items.</remarks>
        public string ContinuationToken { get; set; }
    }
}
