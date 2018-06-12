using System.ComponentModel.DataAnnotations;

namespace DarlMLRestExample
{
    /// <summary>
    /// Used to perform a machine learning run
    /// </summary>
    public class DarlMLData
    {
        /// <summary>
        /// Your DARL code
        /// </summary>
        /// <remarks>Should contain a single ruleset decorated with "supervised" containing only I/O. Outside of the ruleset the "pattern" parameter should be specified, along with mapinputs,mapoutputs and wires. MAP I/O should have paths.</remarks>
        public string code { get; set; }

        /// <summary>
        /// The training data
        /// </summary>
        /// <remarks>this can be in XML or Json. If the former XPath should be used to specify paths in the ruleset. If the latter, JsonPath</remarks>
        public string data { get; set; }

        /// <summary>
        /// Number of sets to use for numeric variables. Only values 3,5,7 and 9 can be specified.
        /// </summary>
        [Range(3,9)]
        public int sets { get; set; }

        /// <summary>
        /// The percent to train on
        /// </summary>
        /// <remarks>Must be between 1 and 100</remarks>
        [Range(1, 100)]
        public int percentTrain { get; set; }

        /// <summary>
        /// email to send results
        /// </summary>
        /// <remarks>Because Machine learning can be CPU intensive training is performed via a queue in a secondary process. Results and the mined DARL will be emailed to this address.</remarks>
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        /// <summary>
        /// A name to identify the job in the returned email
        /// </summary>
        public string jobName { get; set; }

    }
}
