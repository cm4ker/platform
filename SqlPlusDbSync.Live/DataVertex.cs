using GraphX.PCL.Common.Models;

namespace SqlPlusDbSync.Live
{
    public class DataVertex : VertexBase
    {
        /// <summary>
        /// Some string property for example purposes
        /// </summary>
        public string Text { get; set; }

        public string Name { get; set; }

        public int ImageId { get; set; } = 3;

        #region Calculated or static props

        public override string ToString()
        {
            return Text;
        }

        #endregion

        /// <summary>
        /// Default parameterless constructor for this class
        /// (required for YAXLib serialization)
        /// </summary>
        public DataVertex() : this(string.Empty)
        {
        }



        public DataVertex(string text = "")
        {
            Text = text;
        }
    }
}