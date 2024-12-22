import {themes as prismThemes} from 'prism-react-renderer';
import type {Config} from '@docusaurus/types';
import type * as Preset from '@docusaurus/preset-classic';
import { remarkKroki } from 'remark-kroki';
import rehypeRaw from 'rehype-raw';

const organizationName = "mt89vein";
const projectName = "BookLibrary";
const baseUrl = `/${projectName}/`;

// This runs in Node.js - Don't use client-side code here (browser APIs, JSX...)

const config: Config = {
  title: 'BookLibrary',
  tagline: 'Docs for BookLibrary',
  favicon: 'img/favicon.ico',
  url: `https://${organizationName}.github.io`,
  baseUrl,
  organizationName,
  projectName,
  onBrokenLinks: 'throw',
  onBrokenMarkdownLinks: 'warn',

  // Even if you don't use internationalization, you can use this field to set
  // useful metadata like html lang. For example, if your site is Chinese, you
  // may want to replace "en" with "zh-Hans".
  i18n: {
    defaultLocale: 'en',
    locales: ['en'],
  },

  presets: [
    [
      'classic',
      {
        docs: {
          sidebarPath: './sidebars.ts',
          editUrl:
            `https://github.com/${organizationName}/${projectName}/tree/master/`,
          remarkPlugins: [
            [
              remarkKroki,
              {
                server: 'https://kroki.io',
                target: 'mdx3',
                output: 'inline-svg'
              }
            ]
          ],
          rehypePlugins: [
            [
              rehypeRaw,
              {
                passThrough: [
                  'mdxFlowExpression',
                  'mdxJsxFlowElement',
                  'mdxJsxTextElement',
                  'mdxTextExpression',
                  'mdxjsEsm'
                ]
              }
            ]
          ]
        },
        theme: {
          customCss: './src/css/custom.css',
        },
      } satisfies Preset.Options,
    ],
  ],

  themeConfig: {
    // Replace with your project's social card
    image: 'img/docusaurus-social-card.jpg',
    navbar: {
      title: 'BookLibrary',
      items: [
        {
          href: 'https://github.com/mt89vein/BookLibrary',
          label: 'GitHub',
          position: 'right',
        },
      ],
    },
    footer: {
      style: 'dark',
      links: [
        {
          title: 'Docs',
          items: [
            {
              label: 'Tutorial',
              to: '/docs/',
            },
          ],
        },
        {
          title: 'Community',
          items: [
            {
              label: 'Telegram',
              href: 'https://t.me/sh_dotnet',
            }
          ],
        },
        {
          title: 'More',
          items: [
            {
              label: 'GitHub',
              href: 'https://github.com/mt89vein/BookLibrary',
            },
          ],
        },
      ],
      copyright: `Copyright © ${new Date().getFullYear()} BookLibrary, Inc. Built with Docusaurus.`,
    },
    prism: {
      theme: prismThemes.github,
      darkTheme: prismThemes.dracula,
    },
  } satisfies Preset.ThemeConfig,
};

export default config;
