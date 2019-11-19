using System.Linq;
using N2.Persistence;

namespace N2.Edit.Workflow.Commands
{
    public class SaveCommand : CommandBase<CommandContext>
    {
        IPersister persister;
        public SaveCommand(IPersister persister)
        {
            this.persister = persister;
        }

        public override void Process(CommandContext state)
        {
            persister.Save(state.Content);
            AddChildrenOfContentItem(state.Content);

            foreach (ContentItem item in state.GetItemsToSave())
            {
                if (item != state.Content)
                {
                    persister.Save(item);
                    AddChildrenOfContentItem(item);
                }
            }
        }

        private void AddChildrenOfContentItem(ContentItem parent)
        {
            // VS2017: Fix to create the children to array first since it might be raised an enumerable exception
            // when changing the list
            var children = parent.Children.ToArray();

            foreach (var templateItemChild in children)
            {
                persister.Save(templateItemChild);

                AddChildrenOfContentItem(templateItemChild);
            }
        }
    }
}
