/****************/
202418944
Bùi Nhật Minh
/****************/

using System;
using System.Collections.Generic;

namespace ProjectManagement
{
    // ==========================================
    // 1. LỚP EMPLOYEE (Lớp cha)
    // ==========================================
    public class Employee
    {
        private string _id = "UNKNOWN";
        private string _fullName = "Unnamed employee";
        private double _baseSalary = 0;

        // Đóng gói dữ liệu (Encapsulation)
        public string Id
        {
            get => _id;
            set { if (!string.IsNullOrWhiteSpace(value)) _id = value; }
        }
        public string FullName
        {
            get => _fullName;
            set { if (!string.IsNullOrWhiteSpace(value)) _fullName = value; }
        }
        public double BaseSalary
        {
            get => _baseSalary;
            set { if (value >= 0) _baseSalary = value; }
        }

        // Nạp chồng Constructor (Constructor Overloading)
        public Employee() { }

        public Employee(string id, string fullName)
        {
            Id = id;
            FullName = fullName;
        }

        public Employee(string id, string fullName, double baseSalary) : this(id, fullName)
        {
            BaseSalary = baseSalary;
        }

        // Nạp chồng phương thức (Method Overloading)
        public void IncreaseSalary(double amount)
        {
            if (amount > 0) BaseSalary += amount;
        }

        public void IncreaseSalary(double value, bool byPercentage)
        {
            if (value > 0)
            {
                if (byPercentage) BaseSalary += BaseSalary * (value / 100);
                else BaseSalary += value;
            }
        }

        // Đa hình (Polymorphism)
        public virtual double CalculateMonthlyCost() => BaseSalary;

        public virtual void DisplayInfo()
        {
            Console.WriteLine($"[Nhân viên] Mã: {Id,-5} | Tên: {FullName,-15} | Lương CB: {BaseSalary}");
        }

        // Destructor để quan sát vòng đời
        ~Employee()
        {
            Console.WriteLine($"[Hủy] Đã giải phóng bộ nhớ Employee: {Id}");
        }
    }

    // ==========================================
    // 2. LỚP SOFTWARE ENGINEER (Kế thừa Employee)
    // ==========================================
    public class SoftwareEngineer : Employee
    {
        private string _primaryLanguage = "Unknown";
        private double _technicalAllowance = 0;

        public string PrimaryLanguage
        {
            get => _primaryLanguage;
            set { if (!string.IsNullOrWhiteSpace(value)) _primaryLanguage = value; }
        }
        public double TechnicalAllowance
        {
            get => _technicalAllowance;
            set { if (value >= 0) _technicalAllowance = value; }
        }

        public SoftwareEngineer(string id, string fullName, string primaryLanguage) 
            : base(id, fullName)
        {
            PrimaryLanguage = primaryLanguage;
        }

        public SoftwareEngineer(string id, string fullName, double baseSalary, string primaryLanguage, double technicalAllowance) 
            : base(id, fullName, baseSalary)
        {
            PrimaryLanguage = primaryLanguage;
            TechnicalAllowance = technicalAllowance;
        }

        public override double CalculateMonthlyCost() => BaseSalary + TechnicalAllowance;

        public override void DisplayInfo()
        {
            Console.WriteLine($"[Kỹ sư PM] Mã: {Id,-5} | Tên: {FullName,-15} | Lương CB: {BaseSalary} | Ngôn ngữ: {PrimaryLanguage} | Phụ cấp: {TechnicalAllowance}");
        }

        ~SoftwareEngineer()
        {
            Console.WriteLine($"[Hủy] Đã giải phóng bộ nhớ SoftwareEngineer: {Id}");
        }
    }

    // ==========================================
    // 3. LỚP PROJECT TEAM (Quan hệ Kết tập - Aggregation)
    // ==========================================
    public class ProjectTeam
    {
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        
        // Liên kết không sở hữu
        public Employee Leader { get; private set; }
        private List<Employee> Members { get; set; } = new List<Employee>();

        public ProjectTeam(string projectCode, string projectName)
        {
            ProjectCode = projectCode;
            ProjectName = projectName;
        }

        public ProjectTeam(string projectCode, string projectName, Employee leader) : this(projectCode, projectName)
        {
            ChangeLeader(leader);
        }

        public bool Contains(string employeeId)
        {
            foreach (var m in Members)
                if (m.Id == employeeId) return true;
            return false;
        }

        public bool AddMember(Employee employee)
        {
            if (employee == null || Contains(employee.Id)) return false;
            Members.Add(employee);
            return true;
        }

        public bool AddMember(Employee employee, bool makeLeader)
        {
            if (employee == null) return false;
            
            bool isNewMember = false;
            if (!Contains(employee.Id))
            {
                Members.Add(employee);
                isNewMember = true;
            }
            
            if (makeLeader) Leader = employee;
            return isNewMember;
        }

        public bool RemoveMember(string employeeId)
        {
            if (Leader != null && Leader.Id == employeeId)
            {
                Console.WriteLine($"[Lỗi] Không được xóa trưởng nhóm {employeeId} khi chưa có người thay thế.");
                return false;
            }

            for (int i = 0; i < Members.Count; i++)
            {
                if (Members[i].Id == employeeId)
                {
                    Members.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public void ChangeLeader(Employee newLeader)
        {
            if (newLeader == null) return;
            AddMember(newLeader); // Đảm bảo vào danh sách nếu chưa có
            Leader = newLeader;
        }

        public double CalculateTotalMonthlyCost()
        {
            double total = 0;
            foreach (var m in Members) total += m.CalculateMonthlyCost();
            return total;
        }

        public void DisplayTeam()
        {
            Console.WriteLine($"\n--- DỰ ÁN: {ProjectCode} - {ProjectName} ---");
            Console.WriteLine($"Trưởng nhóm: {(Leader != null ? Leader.FullName : "Chưa phân công")}");
            Console.WriteLine($"Thành viên ({Members.Count}):");
            foreach (var m in Members) m.DisplayInfo();
            Console.WriteLine("--------------------------------------");
        }

        // Destructor: Không hủy các Employee (Thể hiện quan hệ kết tập)
        ~ProjectTeam()
        {
            Console.WriteLine($"[Hủy] Hủy ProjectTeam {ProjectCode} (Các Employee vẫn tồn tại độc lập).");
        }
    }

    // ==========================================
    // 4. CHƯƠNG TRÌNH KIỂM THỬ (MENU)
    // ==========================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            // Khởi tạo các biến dùng chung cho kịch bản
            Employee emp1 = null, emp2 = null;
            SoftwareEngineer se1 = null, se2 = null;
            ProjectTeam team1 = null;

            while (true)
            {
                Console.WriteLine("\n========== KỊCH BẢN KIỂM THỬ ==========");
                Console.WriteLine("1. Tạo 2 Employee (2 Constructor khác nhau)");
                Console.WriteLine("2. Tạo 2 Software Engineer (2 Constructor khác nhau)");
                Console.WriteLine("3. Tăng lương cố định (Employee 1)");
                Console.WriteLine("4. Tăng lương theo % (Employee 2)");
                Console.WriteLine("5. Tạo nhóm dự án chưa có trưởng nhóm");
                Console.WriteLine("6. Thêm 1 nhân sự vào nhóm (addMember)");
                Console.WriteLine("7. Thêm 1 kỹ sư làm trưởng nhóm (addMember, true)");
                Console.WriteLine("8. Thử thêm lại một thành viên đã tồn tại");
                Console.WriteLine("9. Hiển thị danh sách (Gọi đa hình)");
                Console.WriteLine("10. Tính tổng chi phí nhân sự hằng tháng");
                Console.WriteLine("11. Thử xóa trưởng nhóm hiện tại");
                Console.WriteLine("12. Đổi trưởng nhóm và xóa người cũ");
                Console.WriteLine("13. Tạo nhóm thứ 2, chia sẻ nhân sự (Kết tập)");
                Console.WriteLine("14. Hủy nhóm thứ 2 (Kết thúc khối lệnh)");
                Console.WriteLine("15. Chứng minh nhân sự nhóm 2 vẫn tồn tại");
                Console.WriteLine("0. Thoát");
                Console.Write("Chọn test case (0-15): ");
                
                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        emp1 = new Employee("E01", "Nguyễn Văn A");
                        emp2 = new Employee("E02", "Trần Thị B", 1000);
                        emp1.DisplayInfo();
                        emp2.DisplayInfo();
                        break;
                    case "2":
                        se1 = new SoftwareEngineer("S01", "Lê Văn C", "C#");
                        se2 = new SoftwareEngineer("S02", "Phạm Thị D", 1500, "Java", 500);
                        se1.DisplayInfo();
                        se2.DisplayInfo();
                        break;
                    case "3":
                        if (emp1 == null) { Console.WriteLine("Hãy chạy Test 1 trước!"); break; }
                        Console.WriteLine($"Trước khi tăng: {emp1.BaseSalary}");
                        emp1.IncreaseSalary(200);
                        Console.WriteLine($"Sau khi tăng 200: {emp1.BaseSalary}");
                        break;
                    case "4":
                        if (emp2 == null) { Console.WriteLine("Hãy chạy Test 1 trước!"); break; }
                        Console.WriteLine($"Trước khi tăng: {emp2.BaseSalary}");
                        emp2.IncreaseSalary(10, true); // Tăng 10%
                        Console.WriteLine($"Sau khi tăng 10%: {emp2.BaseSalary}");
                        break;
                    case "5":
                        team1 = new ProjectTeam("P001", "Hệ thống Quản lý");
                        team1.DisplayTeam();
                        break;
                    case "6":
                        if (team1 == null || emp1 == null) { Console.WriteLine("Hãy chạy Test 1 và 5 trước!"); break; }
                        team1.AddMember(emp1);
                        Console.WriteLine("Đã thêm E01 vào nhóm.");
                        team1.DisplayTeam();
                        break;
                    case "7":
                        if (team1 == null || se1 == null) { Console.WriteLine("Hãy chạy Test 2 và 5 trước!"); break; }
                        team1.AddMember(se1, true);
                        Console.WriteLine("Đã thêm S01 và set làm Trưởng nhóm.");
                        team1.DisplayTeam();
                        break;
                    case "8":
                        if (team1 == null || emp1 == null) { Console.WriteLine("Chưa có nhóm hoặc nhân sự!"); break; }
                        bool result = team1.AddMember(emp1);
                        Console.WriteLine($"Thử thêm lại E01. Kết quả (true=thành công, false=đã tồn tại): {result}");
                        break;
                    case "9":
                        if (team1 == null) { Console.WriteLine("Chưa có nhóm!"); break; }
                        team1.DisplayTeam(); // displayInfo() được gọi đa hình bên trong
                        break;
                    case "10":
                        if (team1 == null) { Console.WriteLine("Chưa có nhóm!"); break; }
                        Console.WriteLine($"Tổng chi phí hằng tháng của {team1.ProjectCode}: {team1.CalculateTotalMonthlyCost()}");
                        break;
                    case "11":
                        if (team1 == null || team1.Leader == null) { Console.WriteLine("Nhóm chưa có Trưởng nhóm!"); break; }
                        Console.WriteLine($"Cố gắng xóa trưởng nhóm: {team1.Leader.Id}");
                        team1.RemoveMember(team1.Leader.Id);
                        break;
                    case "12":
                        if (team1 == null || emp2 == null) { Console.WriteLine("Hãy đảm bảo nhóm và E02 đã tạo!"); break; }
                        string oldLeaderId = team1.Leader?.Id;
                        Console.WriteLine("Đổi trưởng nhóm sang E02...");
                        team1.ChangeLeader(emp2);
                        Console.WriteLine($"Xóa trưởng nhóm cũ ({oldLeaderId})...");
                        if (oldLeaderId != null) team1.RemoveMember(oldLeaderId);
                        team1.DisplayTeam();
                        break;
                    case "13":
                        if (emp1 == null) { Console.WriteLine("Chưa có nhân sự E01!"); break; }
                        ProjectTeam team2 = new ProjectTeam("P002", "Dự án phụ", emp1);
                        Console.WriteLine("Đã tạo nhóm P002 với trưởng nhóm là E01 (Nhân sự đang chia sẻ giữa các dự án).");
                        team2.DisplayTeam();
                        break;
                    case "14":
                        Console.WriteLine("Khởi tạo nhóm dự án trong một khối lệnh (scope) nhỏ...");
                        Action createAndDestroy = () => 
                        {
                            ProjectTeam tempTeam = new ProjectTeam("P_TEMP", "Nhóm Tạm Thời");
                            tempTeam.AddMember(emp1);
                            // Hết khối lệnh, tempTeam mất tham chiếu và chờ Garbage Collector thu gom
                        };
                        createAndDestroy();
                        GC.Collect(); // Ép hệ thống dọn rác để gọi Destructor
                        GC.WaitForPendingFinalizers();
                        Console.WriteLine("Khối lệnh đã kết thúc. Team 2 (P_TEMP) đã bị hủy.");
                        break;
                    case "15":
                        if (emp1 == null) { Console.WriteLine("Chưa có nhân sự E01!"); break; }
                        Console.WriteLine("Kiểm tra nhân sự E01: Nếu vẫn hiển thị thông tin nghĩa là nhân sự không bị hủy theo team.");
                        emp1.DisplayInfo();
                        break;
                    case "0":
                        Console.WriteLine("Kết thúc chương trình.");
                        return;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ!");
                        break;
                }
            }
        }
    }
}