using N2.Edit.FileSystem;
using NUnit.Framework;

namespace N2.Tests.Edit.CloudReady
{
    [TestFixture]
    public class FileHelperTests
    {
        [Test]
        public void Test_safe_filename_not_changed()
        {
            var testFileName = "abcdefghijklmnopqrstuvxyz0123456789.jpg";
            var result = FileHelper.GetCloudReadyFileName(testFileName);
            Assert.AreEqual(testFileName, result);
        }

        [Test]
        public void Test_safe_filename_changed_to_lower()
        {
            var testFileName = "ABCDEFGHIJKLMNOPQRSTUVXYZ.jpg";
            var result = FileHelper.GetCloudReadyFileName(testFileName);
            Assert.AreEqual("abcdefghijklmnopqrstuvxyz.jpg", result);
        }

        [Test]
        public void Test_safe_filename_swedish_characters()
        {
            var testFileName = "ÅÄÖåäö.jpg";
            var result = FileHelper.GetCloudReadyFileName(testFileName);
            Assert.AreEqual("aaoaao.jpg", result);
        }

        [Test]
        public void Test_safe_filename_empty_characters()
        {
            var testFileName = "A B C.jpg";
            var result = FileHelper.GetCloudReadyFileName(testFileName);
            Assert.AreEqual("a-b-c.jpg", result);
        }

        [Test]
        public void Test_safe_filename_underscore_to_minus()
        {
            var testFileName = "a_b_c.jpg";
            var result = FileHelper.GetCloudReadyFileName(testFileName);
            Assert.AreEqual("a-b-c.jpg", result);   // Handled bether when using SEO
        }

        [Test]
        public void Test_safe_filename_with_illegal_characters()
        {
            var testFileName = "a!b#c&.jpg";
            var result = FileHelper.GetCloudReadyFileName(testFileName);
            Assert.AreEqual("abc.jpg", result);  
        }

        [Test]
        public void Test_safe_directory_not_changed()
        {
            var testLocalPath = @"korv/apa";
            var result = FileHelper.GetCloudReadyLocalPath(testLocalPath);
            Assert.AreEqual(@"korv/apa", result);   
        }

        [Test]
        public void Test_safe_directory_changed_to_lower()
        {
            var testLocalPath = @"korv/APA";
            var result = FileHelper.GetCloudReadyLocalPath(testLocalPath);
            Assert.AreEqual(@"korv/apa", result);
        }

        [Test]
        public void Test_safe_directory_swedish_characters()
        {
            var testLocalPath = @"korv/ÅÄÖ";
            var result = FileHelper.GetCloudReadyLocalPath(testLocalPath);
            Assert.AreEqual(@"korv/aao", result);
        }

        [Test]
        public void Test_illegal_characters_valid_string()
        {
            var testString = @"abcdefghijklmnopqrstuvxyz0123456789.-";
            var result = FileHelper.RemoveIllegalCharacters(testString, '.', '-');
            Assert.AreEqual(testString, result);
        }

        [Test]
        public void Test_illegal_characters_remove()
        {
            var testString = @"korv!#¤%&/()=.jpg";
            var result = FileHelper.RemoveIllegalCharacters(testString, '.', '-');
            Assert.AreEqual("korv.jpg", result);
        }
    }
}
