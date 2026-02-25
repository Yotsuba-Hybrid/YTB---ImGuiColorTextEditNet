// @ts-check
import { defineConfig } from "astro/config";
import starlight from "@astrojs/starlight";

// https://astro.build/config
export default defineConfig({
  integrations: [
    starlight({
      title: "ImGuiColorTextEditNet",
      description:
        "A multi-line ImGui text editor with syntax highlighting for .NET",
      social: [
        {
          icon: "github",
          label: "GitHub",
          href: "https://github.com/Yotsuba-Hybrid/YTB---ImGuiColorTextEditNet",
        },
      ],
      logo: {
        alt: "ImGuiColorTextEditNet",
        src: "./src/assets/houston.webp",
      },
      customCss: ["./src/styles/custom.css"],
      sidebar: [
        {
          label: "Inicio",
          items: [
            { label: "Introducción", slug: "guides/introduction" },
            { label: "Instalación", slug: "guides/installation" },
            { label: "Quick Start", slug: "guides/quick-start" },
          ],
        },
        {
          label: "Tutoriales",
          items: [
            { label: "Editor Básico", slug: "tutorials/basic-editor" },
            {
              label: "Syntax Highlighting",
              slug: "tutorials/syntax-highlighting",
            },
            { label: "Lenguajes Disponibles", slug: "tutorials/languages" },
            {
              label: "Lenguajes Personalizados",
              slug: "tutorials/custom-languages",
            },
            {
              label: "Breakpoints y Errores",
              slug: "tutorials/breakpoints-errors",
            },
            { label: "Temas y Colores", slug: "tutorials/themes-colors" },
            {
              label: "Input y Keybindings",
              slug: "tutorials/input-keybindings",
            },
          ],
        },
        {
          label: "Avanzado",
          items: [
            { label: "Arquitectura Interna", slug: "advanced/architecture" },
            {
              label: "Crear un Highlighter",
              slug: "advanced/custom-highlighter",
            },
            {
              label: "Integración con ImGui",
              slug: "advanced/imgui-integration",
            },
          ],
        },
        {
          label: "API Reference",
          autogenerate: { directory: "reference" },
        },
      ],
    }),
  ],
});
