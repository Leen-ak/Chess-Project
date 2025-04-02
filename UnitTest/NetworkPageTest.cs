using System.Threading.Tasks;
using DAL;
using Xunit;

namespace UnitTest
{
    public class NetworkPageTest
    {
        readonly IRepository<UserInfo> _repoUser;
        readonly IRepository<Follower> _repoFollower;
        public NetworkPageTest()
        {
            _repoUser = new RepositoryImplementation<UserInfo>();
            _repoFollower = new RepositoryImplementation<Follower>();
        }

        [Fact]
        public async Task TestSuggestUsers()
        {
            NetworkDAO _dao = new NetworkDAO();
            int? testUserId = 2;
            List<UserInfo> suggestUsers = await _dao.GetSuggestedUsers(testUserId!);
            Assert.NotNull(suggestUsers);
            Assert.All(suggestUsers, user =>
            {
                Assert.NotEqual(testUserId, user.Id);
            });
        }
    }
}