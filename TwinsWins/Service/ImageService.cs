namespace TwinsWins.Services
{
    public class ImageService
    {
        private readonly string _imageDirectory;

        public ImageService(string imageDirectory)
        {
            _imageDirectory = imageDirectory;
        }

        public List<ImagePair> GetRandomImagePairs(int count)
        {
            var animalFolders = Directory.GetDirectories(_imageDirectory).OrderBy(_ => Guid.NewGuid()).Take(count).ToList();
            var imagePairs = new List<ImagePair>();

            var random = new Random();

            foreach (var folder in animalFolders)
            {
                var rawImages = Directory.GetFiles(Path.Combine(folder, "raw")).OrderBy(_ => random.Next()).ToList();
                var stylizedImages = Directory.GetFiles(Path.Combine(folder, "stylized")).OrderBy(_ => random.Next()).ToList();

                if (rawImages.Any() && stylizedImages.Any())
                {
                    var rawImage = rawImages.First();
                    var stylizedImage = stylizedImages.First();

                    imagePairs.Add(new ImagePair
                    {
                        ImagePath1 = rawImage,
                        ImagePath2 = stylizedImage
                    });
                }
            }

            return imagePairs.OrderBy(_ => Guid.NewGuid()).ToList(); // Shuffle
        }
    }
}
