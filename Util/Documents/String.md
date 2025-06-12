## StringComparison 
枚举用于指定字符串比较时的文化、大小写和排序规则。它常用于 string.Compare、string.Equals 等方法。
•	用户可见字符串（如排序、显示）：推荐用 CurrentCulture 或 CurrentCultureIgnoreCase。
•	技术标识符（如文件名、代码、协议）：推荐用 Ordinal 或 OrdinalIgnoreCase。
•	跨区域一致性：用 InvariantCulture 或 InvariantCultureIgnoreCase。