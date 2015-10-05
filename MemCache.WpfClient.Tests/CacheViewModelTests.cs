using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace MemCache.WpfClient.Tests
{
    public class CacheViewModelTests
    {
        [Fact]
        public void Items_always_contain_a_new_item()
        {
            var viewModel = new CacheViewModel(Mock.Of<ICacheClient>());

            var items = viewModel.Items;

            Assert.False(items.IsEmpty);
            Assert.IsType<NewCacheItemViewModel>(items.Cast<object>().First());
        }

        [Fact]
        public void Items_loading_on_access()
        {
            var mock = Mock.Of<ICacheClient>(m => m.GetKeys() == GetKeys());
            var viewModel = new CacheViewModel(mock);

            var items = viewModel.Items;

            // 3 + 1 new
            Assert.Equal(4, items.Cast<object>().Count());
        }

        [Fact]
        public void SetCommand_should_call_ICacheClient_Set_on_new_item()
        {
            var mock = new Mock<ICacheClient>();
            var viewModel = new CacheViewModel(mock.Object);

            viewModel.Items.MoveCurrentToFirst();
            var item = (NewCacheItemViewModel)viewModel.Items.CurrentItem;
            item.Key = "abc";
            item.Value = 123;

            viewModel.SetCommand.Execute(null);

            mock.Verify(m => m.Set(It.Is<string>(v => v == "abc"), It.Is<object>(v => Equals(v, 123))), Times.Once);
        }

        [Fact]
        public void SetCommand_can_not_be_executable_if_no_changes_was_made()
        {
            var mock = new Mock<ICacheClient>();
            var viewModel = new CacheViewModel(mock.Object);

            viewModel.Items.MoveCurrentToFirst();
            
            Assert.False(viewModel.SetCommand.CanExecute(null));
        }

        [Fact]
        public void RemoveCommand_should_call_ICacheClient_Remove_on_existing_item()
        {
            var mock = new Mock<ICacheClient>();
            var viewModel = new CacheViewModel(mock.Object);

            var item = (NewCacheItemViewModel)viewModel.Items.CurrentItem;
            item.Key = "abc";
            item.Value = 123;

            viewModel.SetCommand.Execute(null);
            viewModel.Items.MoveCurrentToFirst();
            viewModel.RemoveCommand.Execute(null);

            mock.Verify(m => m.Remove(It.Is<string>(v => v == "abc")), Times.Once);
        }

        [Fact]
        public void RemoveCommand_should_not_call_ICacheClient_Remove_on_new_item()
        {
            var mock = new Mock<ICacheClient>();
            var viewModel = new CacheViewModel(mock.Object);
            
            viewModel.Items.MoveCurrentToFirst();
            viewModel.RemoveCommand.Execute(null);

            mock.Verify(m => m.Remove(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void Should_catch_errors_on_items_loading()
        {
            var mock = new Mock<ICacheClient>();
            mock.Setup(m => m.GetKeys()).Throws(new Exception());

            var viewModel = new CacheViewModel(mock.Object);

            var items = viewModel.Items;

            Assert.False(items.IsEmpty);
        }

        [Fact]
        public void DisconnectCommand_should_call_ICacheClient_Disconnect_and_invoke_callback()
        {
            var mock = new Mock<ICacheClient>();
            var disconnectInvoked = false;
            var viewModel = new CacheViewModel(mock.Object, () => disconnectInvoked = true);

            viewModel.DisconnectCommand.Execute(null);

            mock.Verify(m => m.Disconnect(), Times.Once);
            Assert.True(disconnectInvoked);
        }

        private async Task<IEnumerable<string>> GetKeys()
        {
            return new[] {"a", "b", "c"};
        } 
    }
}
