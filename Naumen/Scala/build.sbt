lazy val root = (project in file("."))
  .enablePlugins(PlayScala)
  .settings(
    name := "NaumenScala",
    version := "0.1",
    scalaVersion := "2.12.8",
    serverPort := 8888,
    libraryDependencies ++= Seq(
      guice,
      "org.scalatestplus.play" %% "scalatestplus-play" % "5.0.0" % Test
    ),
    scalacOptions ++= Seq(
      "-feature",
      "-deprecation",
      "-Xfatal-warnings"
    )
  )

PlayKeys.devSettings += "play.server.http.port" -> "8888"