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
            foreach (var templateItemChild in parent.Children)
            {
                persister.Save(templateItemChild);

                AddChildrenOfContentItem(templateItemChild);
            }
        }
    }
}
