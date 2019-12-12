using N2.Edit.FileSystem;
using NUnit.Framework;

namespace N2.Tests.Edit.CloudReady
{
    [TestFixture]
    public class FileHelperTests
    {
        [Test]
        public void Test_safe_filename_for_cloud_not_changed()
        {
            var testFileName = "abcdefghijklmnopqrstuvxyz0123456789.jpg";
            var result = FileHelper.GetSafeFileNameToUseAlsoForCloudStorage(testFileName);
            Assert.AreEqual(testFileName, result);
        }

        [Test]
        public void Test_safe_filename_for_cloud_changed_to_lower()
        {
            var testFileName = "ABCDEFGHIJKLMNOPQRSTUVXYZ.jpg";
            var result = FileHelper.GetSafeFileNameToUseAlsoForCloudStorage(testFileName);
            Assert.AreEqual("abcdefghijklmnopqrstuvxyz.jpg", result);
        }

        [Test]
        public void Test_safe_filename_for_cloud_swedish_characters()
        {
            var testFileName = "ÅÄÖåäö.jpg";
            var result = FileHelper.GetSafeFileNameToUseAlsoForCloudStorage(testFileName);
            Assert.AreEqual("aaoaao.jpg", result);
        }

        [Test]
        public void Test_safe_filename_for_cloud_empty_characters()
        {
            var testFileName = "A B C.jpg";
            var result = FileHelper.GetSafeFileNameToUseAlsoForCloudStorage(testFileName);
            Assert.AreEqual("a-b-c.jpg", result);
        }

        [Test]
        public void Test_safe_filename_for_cloud_underscore_to_minus()
        {
            var testFileName = "a_b_c.jpg";
            var result = FileHelper.GetSafeFileNameToUseAlsoForCloudStorage(testFileName);
            Assert.AreEqual("a-b-c.jpg", result);   // Handled bether when using SEO
        }
    }
}
