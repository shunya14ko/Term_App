using TermApp.Models;

namespace TermApp.Data.Repositories
{
    //全取得、追加、更新、削除、検索を定義
    public class NoteGroupRepository
    {
        public NoteGroupRepository() { }
        //全取得
        public List<Term> GetAll()
        {
            LinQ
            SEKECT * FROM "Note_Gorep";

        }
    }
}
