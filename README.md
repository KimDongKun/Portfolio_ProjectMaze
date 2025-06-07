# Portfolio_ProjectMaze
![image](https://github.com/user-attachments/assets/74c7704f-848b-4a31-938c-5d5d7385e03d)


랜덤 맵 생성(MapGenerate) 구현
- 맵 생성시 Grid 방식을 통하여 (가로x세로x높이) 원하는 규격의 랜덤 맵 생성.
- Prefab으로 구성된 room을 미리 준비하여 레고 블록처럼 설치 할 수 있도록 Prefab 세팅.
- room의 연결점과 Grid규격 내에 설치 유무 확인후 블록 생성.
  
Server에 생성된 GameObject를 Client 동기화
- Host(Server) 에서 맵 생성시 게임에 참여하는 Client에 Spawn으로 맵 생성 로직 구현



![image](https://github.com/user-attachments/assets/627ace33-39a0-4943-9976-43a9d6ec1e58)


맵 생성시 플레이어 추적할 NavMesh 생성
- FindObjectsByType을 활용하여 게임에 참여하는 Player List를 참조
- 탐지거리 내 제일 가까운 Player를 추적 시작.
- 탐지거리내 없을 시 ResetPath()를 통해 Nav경로 초기화.
  
Enemy 애니메이션
- (Idle – 이동(추적) - 공격) 상태별 애니메이션 설정.




![image](https://github.com/user-attachments/assets/33fb5c7f-905d-4504-ac0b-cca2ecc5d82a)


Steamworks를 통해 Host방식의 서버 구현
- Steamworks에서 지원하는 SteamMachmaking를 활용하여 Lobby구현
- Lobby를 생성하면서 출력된 LobbyId를 활용하여 Room 입장 구현
- Player가 Host의 역할을 수행하여 별도의 서버 관리가 필요 없음.
  
게임 내 lobby 리스트 구현
- 새로고침 버튼을 구현하여 lobbyList를 갱신
- Room 설정을 통해 (친구or초대만 가능) 유무 체크기능 구현.

