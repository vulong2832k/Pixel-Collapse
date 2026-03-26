    Sử dụng phiên bản Unity 2022.3.21f1 mở Project và chạy game. có 1 nút Play ở giữa màn hình để bắt đầu màn chơi.
    Game được thiết kế bằng 3 tool: Level Tool, Pixel Editor, Grid Map Generator. Được lưu trữ bằng JSON và load bằng Script LevelLoader; Có hệ thống MultiPool để chứa
tất cả những GameObject trong game; GameManager quản lý hệ thống XP của màn chơi, lấy dữ liệu từ LevelLoader, UpgradeUIManager quản lý Panel sau khi người chơi Upgrade
và những Skill được upgrade trong đó.
    Hướng dẫn sử dụng các tool:
      GridMapGenerator: Tạo 1 GO mới và gán Script GeneratorEditor vào, khi hoàn thành thì xóa GO đó đi. Chọn trục X, Z và mặc định Size là 1 sau đó Generator new map, đặt tên map là Map_1 (nhiều level thì chỉ thay đổi số phía sau), đặt những Prefab
liên quan về Wall, Ground vào bảng danh sách chọn, Chọn chế độ Paint Ground hoặc Build Wall để làm việc với map, chuột trái-tạo mới Prefab, chuột phải-xóa Prefab,
Save Map để tự tạo 1 file JSON theo mẫu "Map_{number}".
      LevelTool: Trên thanh công cụ của Unity, bấm LevelTool để mở thanh công cụ. Khi đã có danh sách Map thì bấm RefreshList để lấy danh sách, chọn Map để design 
sau đó bấm Create Level From Map. Chỉnh sửa: Máu Cube trước khi bị tách khỏi khối, Vùng Spawn các Object, Add Pixel JSON Slot để đưa danh sách cái GameObject được vẽ 
bằng JSON, sau đó Add vị trí các TowerEmpty, chỗ Tower Prefab chọn Tower Empty. Chọn Số lượng điểm hoàn thành màn chơi và điểm mỗi lần Upgrade. Save Level Data để lưu
file dưới dạng JSON, số Level_{number} được lấy từ Map_{number}. Lưu ý: Phải chọn Map để design sau đó bấm Create Level From Map để không bị chèn nhầm File.
      PixelEditorWindow: Trên thanh công cụ của Unity, bấm PixelEditor để mở thanh công cụ Dùng 1 Material được tạo thành từ Shader Graph sử dụng Vertex Color. Sau đó
chọn Width, Height và bấm Reset Grid. Chọn Paletee là 1 Pixel Data được tạo từ Scriptable Object. vùng Grid(Top-Down, Z-X Plane) là vùng để vẽ vật thể. Selected Color
dùng để thay đổi màu sắc mình vẽ vật thể đó, chuột trái nhấn hoặc đè để vẽ, chuột phải để xóa. Sau đó đặt tên vật thể và bấm Save Prefab để lưu. Có thể Refresh List lại
để lấy danh sách vật thể đã từng làm để thay đổi tùy thiết kế.
    Những điều muốn làm sắp tới: Thêm được nhiều các nâng cấp cũng như đa dạng các tháp phá hủy, âm thanh, hiệu ứng hoàn thiện hơn, tối ưu giao diện cho tất cả hệ máy,
thêm được nhiều màn chơi và nhiều vật thể hơn để người chơi thỏa sức phá hủy. Nâng cao trải nghiệm người dùng, Làm UI bằng DOTWeen để game mượt mà hơn.
