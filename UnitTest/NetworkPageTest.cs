using System.Threading.Tasks;
using DAL;
using Xunit;

namespace UnitTest
{
    public class NetworkPageTest
    {
        readonly IRepository<UserInfo> _repoUser;
        readonly IRepository<Follower> _repoFollower;
        NetworkDAO _dao; 
        public NetworkPageTest()
        {
            _repoUser = new RepositoryImplementation<UserInfo>();
            _repoFollower = new RepositoryImplementation<Follower>();
            _dao = new NetworkDAO();


        }

        [Fact]
        public async Task TestSuggestUsers()
        {
            int? testUserId = 2;
            List<UserInfo> suggestUsers = await _dao.GetSuggestedUsers(testUserId!);
            Assert.NotNull(suggestUsers);
            Assert.All(suggestUsers, user =>
            {
                Assert.NotEqual(testUserId, user.Id);
            });
        }

        [Fact]
        public async Task GetUserInfoTest()
        {

            Follower user = new Follower
            {
                FollowerId = 1,
                FollowingId = 2,
                Status = "Pending"
            };

            int? userId = await _dao.AddUser(user!);
            Assert.True(userId > 0);
        }

        [Fact]
        public async Task UpdateStatusTest()
        {
            Follower user = new Follower
            {
                Id = 1,
                Status = "Accepted"
            };

            var result = await _dao.UpdateFollowStatus(user!);
            Assert.Equal(UpdateStatus.Ok, result);
        }
    }
}