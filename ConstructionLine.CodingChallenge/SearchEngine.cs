using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace ConstructionLine.CodingChallenge
{
    public class SearchEngine
    {
        private readonly List<Shirt> _shirts;

        public SearchEngine(List<Shirt> shirts)
        {
            _shirts = shirts;
            // TODO: data preparation and initialisation of additional data structures to improve performance goes here.         

        }

        public SearchResults Search(SearchOptions options)
        {
            // Intitialize Search result
            SearchResults results = new SearchResults()
            {
                ColorCounts = new List<ColorCount>(),
                SizeCounts = new List<SizeCount>(),
                Shirts = new List<Shirt>()
            };

            List<Shirt> colourMatchedShirts = new List<Shirt>();

            if (options != null)
            {
                foreach (Color color in options.Colors)
                {
                    ColorCount colorCount = new ColorCount() { Color = color, Count = 0 };

                    foreach (Size size in Size.All)
                    {
                        colourMatchedShirts = _shirts.FindAll(item => (item.Color.Name.Equals(color.Name)) && (item.Size.Name.Equals(size.Name)));

                        // Search when sizes are specified 
                        if (options.Sizes.Contains(size) && options.Sizes.Count > 0)
                        {
                            if (colourMatchedShirts.Count > 0)
                            {
                                colorCount.Count += colourMatchedShirts.Count;
                                results.Shirts.AddRange(colourMatchedShirts);
                                SizeCount sizeCount = results.SizeCounts.Find(item => item.Size.Name.Equals(size.Name));
                                if (sizeCount == null)
                                {
                                    results.SizeCounts.Add(new SizeCount { Size = size, Count = colourMatchedShirts.Count });
                                }
                                else
                                {
                                    sizeCount.Count++;
                                }
                            }
                        }
                        else if (options.Sizes.Count == 0)
                        {
                            // Count all sizes
                            colorCount.Count += colourMatchedShirts.Count;
                        }
                    }
                    results.ColorCounts.Add(colorCount);
                }
            }

            return results;
        }

        public SearchResults LookupSearch(SearchOptions options)
        {
            //Intitialize search result
            SearchResults results = new SearchResults()
            {
                ColorCounts = new List<ColorCount>(),
                SizeCounts = new List<SizeCount>(),
                Shirts = new List<Shirt>()
            };

            // Convert to Lookup 
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var shirtsLookup = _shirts.ToLookup(x => new { x.Color, x.Size });
            sw.Stop();
            results.Milliseconds = sw.ElapsedMilliseconds;

            //Search  
            if (options != null)
            {
                foreach (Color color in options.Colors)
                {
                    ColorCount colorCount = new ColorCount() { Color = color, Count = 0 };

                    foreach (Size sz in Size.All)
                    {
                        var colourMatchedShirts = shirtsLookup[new { Color = color, Size = sz }];
                        if (options.Sizes.Contains(sz) && options.Sizes.Count > 0)
                        {
                            if (colourMatchedShirts.Count() > 0)
                            {
                                colorCount.Count += colourMatchedShirts.Count();
                                results.Shirts.AddRange(colourMatchedShirts.ToList<Shirt>());
                                SizeCount sizeCount = results.SizeCounts.Find(item => item.Size.Name.Equals(sz.Name));
                                if (sizeCount == null)
                                {
                                    results.SizeCounts.Add(new SizeCount { Size = sz, Count = colourMatchedShirts.Count() });
                                }
                                else
                                {
                                    sizeCount.Count++;
                                }
                            }
                        }
                        else if (options.Sizes.Count == 0)
                        {
                            colorCount.Count += colourMatchedShirts.Count();
                        }
                    }
                    results.ColorCounts.Add(colorCount);
                }
            }
            return results;
        }        
    }
}