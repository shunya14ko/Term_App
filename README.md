# Term_App

Blazor + MySQL で構築した IT 用語辞典アプリです。グループと用語を整理しながら学習状況を把握できます。

## 主な機能
- ダッシュボードで登録済み用語や残債（未定義の用語）を可視化
- グループ・サブグループの作成と一覧表示
- Bootstrap ベースのレスポンシブ UI
- MySQL との接続に必要なテーブルを自動生成

## セットアップ
1. `appsettings.Development.json` などに接続文字列 `ConnectionStrings:TermAppDb` を設定してください。
   ```json
   {
     "ConnectionStrings": {
       "TermAppDb": "Server=localhost;Port=3306;Database=term_app;User=root;Password=YOUR_PASSWORD;"
     }
   }
   ```
   または環境変数 `ConnectionStrings__TermAppDb` を利用して接続情報を外部化できます。
2. 依存パッケージを復元したのち、以下でアプリを起動します。
   ```bash
   dotnet run --project TermApp
   ```

初回接続時に `term_groups` と `terms` テーブルが自動生成されます。既存のスキーマがある場合は必要に応じて修正してください。

## 今後の予定
- 用語一覧ページの実装
- FullText 検索対応
- UI カスタマイズ
- メモ機能強化

## 作者
舜哉 / Shunya14ko
