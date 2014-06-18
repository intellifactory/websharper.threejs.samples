﻿namespace Site
 
open IntelliFactory.WebSharper

[<JavaScript>]
module Maze =
    open IntelliFactory.WebSharper.ThreeJs
    open IntelliFactory.WebSharper.JQuery
    open IntelliFactory.WebSharper.Dom

    let Main a =
        let A = [
            [1; 1; 1; 1; 1];
            [1; 0; 0; 0; 1];
            [1; 0; 1; 0; 0];
            [1; 0; 0; 0; 1];
            [1; 1; 1; 1; 1]
        ]
    
        let player = new THREE.Mesh(
                         new THREE.BoxGeometry(1., 1., 1.),
                         new THREE.MeshBasicMaterial(
                             MeshBasicMaterialConfiguration(
                                 Color = "red"
                             )
                         )
                     )
    
        let movePlayer (a, b) =
            if A.[int player.Position.Z + b].[int player.Position.X + a] <> 1
            then
                player.Position.X <- player.Position.X + float a
                player.Position.Z <- player.Position.Z + float b
       
        let renderer = new THREE.WebGLRenderer()

        renderer.SetSize(640, 360)
        renderer.SetClearColor("white")

        JQuery.Of(a :> Dom.Node).Append(renderer.DomElement) |> ignore

        let scene = new THREE.Scene()

        A
        |> List.iteri (fun i row ->
            row
            |> List.iteri (fun j a ->
                if a = 1
                then
                    let wall = new THREE.Mesh(
                                    new THREE.BoxGeometry(1., 1., 1.),
                                    new THREE.MeshNormalMaterial()
                                )

                    wall.Position.X <- float j
                    wall.Position.Z <- float i

                    scene.Add(wall)
            )
        )

        player.Position.X <- 1.
        player.Position.Z <- 2.

        scene.Add(player)

        Document.Current.AddEventListener("keyup", (fun (e : Event) ->
            let ke = e :?> KeyboardEvent

            movePlayer (match ke.KeyIdentifier with
                        | "Up" ->
                            (0, -1)
                        | "Right" ->
                            (1, 0)
                        | "Down" ->
                            (0, 1)
                        | "Left" ->
                            (-1, 0)
                        | _ ->
                            (0, 0)
                        )
        ), false)

        let camera = new THREE.PerspectiveCamera(45., 16./9.)
        let controls = new THREE.TrackballControls(camera)

        camera.Position.Z <- 10.

        //---
        //Render loop
        let rec frame () =
            renderer.Render(scene, camera)
            controls.Update()

        Animations.current := frame

        Animations.startIfNotStarted()
        //---

    let Sample =
        Samples.Build()
            .Id("Maze")
            .FileName(__SOURCE_FILE__)
            .Keywords(["game"])
            .Render(Main)
            .Create()