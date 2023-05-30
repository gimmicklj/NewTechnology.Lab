using System;

namespace WebApi.Entity
{
    /// <summary>
    /// 待办事项
    /// </summary>
    public class TodoItem
    {
        public long Id { get; set; }
        
        /// <summary>
        /// 事项名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsComplete { get; set; }

    }

}
