using System.Threading.Tasks;
using DAL;
using Xunit;

namespace UnitTest
{
    public class LinqTests
    {
        readonly IRepository<UserInfo> _repoUser;
        readonly IRepository<Follower> _repoFollower;
        public LinqTests()
        {
            _repoUser = new RepositoryImplementation<UserInfo>();
            _repoFollower = new RepositoryImplementation<Follower>();
        }

        [Fact]
        public async Task Test1()
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