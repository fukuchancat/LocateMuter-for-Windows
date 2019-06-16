# LocateMuter for Windows
大学でのミュート忘れ事故が多発していたのを何とかするためにPowerShellで自動ミュート機能を作った…

[そんなツイート](https://twitter.com/m_fukuchan/status/869105790690279426)がやけにウケて、
ついには[窓の杜さんに紹介される](https://forest.watch.impress.co.jp/docs/serial/yajiuma/1062727.html)までに至ったことから、
ちゃんとしたアプリとして作り直そうとしているもの。

![image](https://user-images.githubusercontent.com/19220989/59559316-364f0b00-903f-11e9-96e6-344aea860afc.png)

ただWindows Sensor and Location APIによる位置情報の精度があまりよろしくなく、開発は停滞中。

Google Geolocation APIにも対応していて、そちらは多少マシな精度だが…？
