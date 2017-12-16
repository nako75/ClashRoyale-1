namespace ClashRoyale.Server.Files.Csv.Logic
{
    internal class GambleChestData : CsvData
    {
		/// <summary>
        /// Initializes a new instance of the <see cref="GambleChestData"/> class.
        /// </summary>
        /// <param name="CsvRow">The row.</param>
        /// <param name="CsvTable">The data table.</param>
        public GambleChestData(CsvRow CsvRow, CsvTable CsvTable) : base(CsvRow, CsvTable)
        {
            // GambleChestData.
        }

        /// <summary>
        /// Called when all instances has been loaded for initialized members in instance.
        /// </summary>
		internal override void LoadingFinished()
		{
	    	// LoadingFinished.
		}
	
        internal int GoldPrice
        {
            get; set;
        }

        internal string Location
        {
            get; set;
        }

    }
}