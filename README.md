# 🖊️ Inkspire (Inkspire, inspired by you)
#### 내가 원하는 나만의 스토리 게임 Inkspire, inspired by you.
<div align="center">

![title](https://github.com/EWHA-Inkspire/inkspire-BE/assets/90602694/23760251-b834-4528-8cf9-05ae98616823)

[![Hits](https://hits.seeyoufarm.com/api/count/incr/badge.svg?url=https%3A%2F%2Fgithub.com%2FEWHA-Inkspire%2Finkspire_front%2F&count_bg=%23A3B7FF&title_bg=%23555555&icon=&icon_color=%23E7E7E7&title=hits&edge_flat=false)](https://hits.seeyoufarm.com)

</div>


## 🦕 팀원 소개
<div align="center">

|      개발(FE), UI         |      개발(FE)       |          개발(FE,BE)         |                                                                                                            
| :------------------------------------------------------------------------------: | :---------------------------------------------------------------------------------------------------------------------------------------------------: | :---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------: |
| <img width="160px" src="https://avatars.githubusercontent.com/u/95266994?v=4"/> | <img width="160px" src="https://avatars.githubusercontent.com/u/96673257?v=4"/> | <img width="160px" src="https://avatars.githubusercontent.com/u/90602694?v=4"/> |
| [@billy0904](https://github.com/billy0904) | [shl4869](https://github.com/shl4869) | [@SuHyeon00](https://github.com/SuHyeon00) |
| 이가빈 | 이소민 | 오수현 |

</div>

<br/>

## 🔮 About Project
> **Ewha Capstone Design Project** <br/>
> 개발 기간: 2023.09.01 ~ 2024.06.06

![about_project](https://github.com/EWHA-Inkspire/inkspire-BE/assets/90602694/d2b501b8-8366-47e5-9fb3-195962ae0810)

<br/>

## Getting Started
- 제출된 **apk 파일을 설치**하거나 **프로젝트를 직접 빌드**하여 실행할 수 있습니다. 
### Build:
- 현재 레포지토리를 클론하여 로컬에서 빌드할 수 있습니다.
```bash
git clone https://github.com/EWHA-Inkspire/inkspire_front.git
```
- 빌드를 진행하기 위해서는 Unity Hub와 Unity, VS Code\(혹은 VisualStudio)의 설치가 필요합니다.
  - [Unity Hub 다운로드](https://unity.com/kr/download)
  - [Unity 다운로드](https://unity.com/kr/releases/editor/archive) \(버전명 2021.3.15f1)

0. 프로젝트 폴더 내 두 C# 스크립트에 localhost 주소와 gpt-api key 추가 \(그로쓰 Git-Repo URL 폼에 별도 제출)
![0-1](https://github.com/EWHA-Inkspire/inkspire_front/assets/96673257/d28cc3ee-2b4f-4f9f-b99a-6af6f20968bc)
![0-2](https://github.com/EWHA-Inkspire/inkspire_front/assets/96673257/1d766830-db73-486b-a7d2-5d129d80d097)

1. Unity Hub 실행 및 좌측 탭에서 Project 선택 후 우상단 "Add"버튼 옆 역삼각형 버튼 클릭
![1](https://github.com/EWHA-Inkspire/inkspire_front/assets/96673257/3af464a7-75a4-4858-a3f0-4a045af340a3)

2. "Add project from disk"버튼 클릭 후 프로젝트 레포지토리 폴더 내 "InkSpire" 폴더 선택, "Add Project"버튼 클릭
![2](https://github.com/EWHA-Inkspire/inkspire_front/assets/96673257/82dc8cf6-0391-4117-b1c0-17a96b916331)
![3](https://github.com/EWHA-Inkspire/inkspire_front/assets/96673257/9fad50b0-ca95-4c54-89a2-c3c1fd00c3ea)

3. Unity Hub에 추가된 "InkSpire" 프로젝트 실행
![4](https://github.com/EWHA-Inkspire/inkspire_front/assets/96673257/48277834-bf36-42e9-b4a3-b3903219d5ab)

4. Unity 상단 메뉴 "File" 중 "Build Settings..." 선택
![5](https://github.com/EWHA-Inkspire/inkspire_front/assets/96673257/20c5c102-6065-49f2-bfa4-ce62cadd00e1)

5. 팝업창의 "Android" 선택, "Switch Platform" 버튼 클릭 후 팝업창을 닫으면 플랫폼 변경 Unity에서 자동 진행
![6](https://github.com/EWHA-Inkspire/inkspire_front/assets/96673257/a3dd3c68-2ffa-4a6d-a8de-5d241eff7ca3)

6. 다시 항목4의 빌드 셋팅 창을 연 후 "Build" 버튼 클릭 후 apk파일을 저장할 경로 지정
![7](https://github.com/EWHA-Inkspire/inkspire_front/assets/96673257/44519e8d-00b3-41c9-ae79-e139bfd8d147)

### Install:
1. 안드로이드OS 모바일 환경에 apk파일 다운로드 후 실행
2. 팝업: "이 앱을 설치하시겠습니까?" -> "설치" 선택
3. 팝업: "출처를 알 수 없는 앱 설치" -> "무시하고 설치" 선택
4. Google Play 프로텍트 팝업: "앱 검사 권장됨" -> "세부정보 더보기" 클릭 -> "검사 없이 설치" 선택
5. app 설치 확인 후 실행
<br/>


## 📪 기능 소개
### 1. 개인화된 세계관 생성
플레이어가 입력한 장르와 시간/공간적 배경에 따라 각기 다른 세계관을 기반으로 한 게임을 생성한다.

<img width="22%" height="22%" alt="장르 선택" src="https://github.com/EWHA-Inkspire/inkspire-BE/assets/90602694/5d9d7925-1694-4d26-8456-6bc0c56d3464">
<img width="22%" height="22%" alt="캐릭터 스탯 설정" src="https://github.com/EWHA-Inkspire/inkspire-BE/assets/90602694/baca5948-d556-4b07-9a89-ad9fdc68a50f">

### 2. 실시간 대화형 기반 게임 진행
플레이어의 행동 지문에 따라 실시간으로 스토리라인이 변화하며 게임을 플레이할 수 있다.

<img width="22%" height="22%" alt="인트로" src="https://github.com/EWHA-Inkspire/inkspire-BE/assets/90602694/e3a1d9bc-22ff-4c13-8b83-ef4dbdf9cf22">
<img width="22%" height="22%" alt="대화 진행" src="https://github.com/EWHA-Inkspire/inkspire-BE/assets/90602694/0c525b82-4173-4b4a-bdc2-b38f1cea3184">

### 3. 주사위 / 전투 이벤트
플레이어의 행동이 특정 퀘스트의 트리거에 부합할 경우 랜덤성을 부여한 주사위 이벤트 혹은 전투 이벤트가 발동한다.<br/>
하나의 퀘스트를 성공하면 아이템을 획득하며 해당 아이템은 인벤토리 창에서 확인할 수 있다.

<img width="22%" height="22%" alt="주사위" src="https://github.com/EWHA-Inkspire/inkspire-BE/assets/90602694/64167af2-5882-4305-b067-869bace779f6">
<img width="22%" height="22%" alt="전투" src="https://github.com/EWHA-Inkspire/inkspire-BE/assets/90602694/d2234a5a-82cb-4f23-ac20-7602dd1df67f">
<img width="22%" height="22%" alt="인벤토리" src="https://github.com/EWHA-Inkspire/inkspire-BE/assets/90602694/aa1c7524-b10c-48bc-83de-3f70a1b48173">

### 4. 에필로그
각 챕터들의 목표 달성 여부에 따라 각기 다른 에필로그와 이미지가 생성된다.

<img width="22%" height="22%" alt="에필로그" src="https://github.com/EWHA-Inkspire/inkspire-BE/assets/90602694/69e41aa1-3991-4a10-a38c-0d8d2df69c5b">

### 5. 게임 내용 아카이빙
하나의 스토리 게임을 진행하며 얻은 업적이나 플레이 내용을 기록하여 다시 확인할 수 있다.

<img width="22%" height="22%" alt="탐험목록" src="https://github.com/EWHA-Inkspire/inkspire-BE/assets/90602694/214f21ee-feb7-4ef9-9d24-ab44431735aa">
<img width="22%" height="22%" alt="프로필" src="https://github.com/EWHA-Inkspire/inkspire-BE/assets/90602694/c5ca56dc-0ebc-4239-812d-28ac6d8c42c9">

<br/>

## 💡 유저 플로우
#### 게임 생성
- 플레이어가 장르와 시공간적 배경을 입력하면, 스크립트 매니저에서 이를 바탕으로 가장 먼저 세계관을 생성
- 세계관 정보를 바탕으로 NPC, 목표, 세계관에 어울리는 이미지 병렬적으로 생성
- 목표 생성이 완료된 후 이전 정보들을 토대로 게임에 필요한 나머지 요소들을 생성

<img width=90% alt="11" src="https://github.com/EWHA-Inkspire/inkspire-BE/assets/90602694/9e29ce5d-4ca6-4653-8cb7-c118e92431b7">

#### 게임 진행
- 사용자가 행동 지문을 입력하면, 현재 플레이어가 위치한 장소의 퀘스트 존재와 달성 여부를 기준으로 이벤트 분기
- 전투와 주사위 이벤트 분기로 진행되거나, 입력 지문에 따라 GPT가 각기 다른 방향의 스토리를 생성하며 진행
<img width=90% alt="15" src="https://github.com/EWHA-Inkspire/inkspire-BE/assets/90602694/bdc7b861-58cb-4c54-ac37-a61314ed77c3">


## 🐈 Stacks

### Environment
![Unity](https://img.shields.io/badge/Unity-100000?style=for-the-badge&logo=unity&logoColor=white)
![Intellij](https://img.shields.io/badge/IntelliJ_IDEA-EA4065.svg?style=for-the-badge&logo=intellij-idea&logoColor=white)
![Git](https://img.shields.io/badge/Git-F05032?style=for-the-badge&logo=Git&logoColor=white)
![Github](https://img.shields.io/badge/GitHub-181717?style=for-the-badge&logo=GitHub&logoColor=white)             

### FrontEnd
![C#](https://img.shields.io/badge/-C%23-000000?style=for-the-badge&logo=Csharp&logoColor=white)

### BackEnd
![SpringBoot](https://img.shields.io/badge/Springboot-6DB33F?style=for-the-badge&logo=Springboot&logoColor=white)
![JPA](https://img.shields.io/badge/jpa-6DB33F?style=for-the-badge&logo=jpa&logoColor=white)
![MySQL](https://img.shields.io/badge/MySQL-005C84?style=for-the-badge&logo=mysql&logoColor=white)

### CI/CD
![GithubActions](https://img.shields.io/badge/Github_Actions-2088FF?style=for-the-badge&logo=githubactions&logoColor=white)

### Deploy
![AWS EC2](https://img.shields.io/badge/AWS_EC2-FF9900?style=for-the-badge&logo=amazonec2&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=Docker&logoColor=white)

### Communication
![Notion](https://img.shields.io/badge/Notion-000000?style=for-the-badge&logo=Notion&logoColor=white)
![Figma](https://img.shields.io/badge/Figma-F24E1E?style=for-the-badge&logo=figma&logoColor=white)


## 🛠️ 아키텍쳐
<div align="center">
<img width=600 alt="architecture" src="https://github.com/EWHA-Inkspire/inkspire-BE/assets/90602694/018d64dc-5a51-492d-b722-009c026b4cbe">
</div>

### ER Diagram
<div align="center">
<img alt="erd" src="https://github.com/EWHA-Inkspire/inkspire-BE/assets/90602694/e98b3bac-b10c-440f-8cf6-ab1252ce2f3d">
</div>

## 🖇️ Docs

🫧 [기획](https://wiry-elderberry-3bf.notion.site/Inkspire-81453fd99d9548a381f6a23cf9925279?pvs=4)

👩🏻‍💻 [그라운드 룰](https://wiry-elderberry-3bf.notion.site/217575c6650a40c79933bbf52f3b1f8c?pvs=4)

📁 [API 명세서](https://wiry-elderberry-3bf.notion.site/API-cd2e5b8ae6dd4263a83f8b04bbefe6e6?pvs=4)

<br/>
